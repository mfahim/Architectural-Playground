using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MicroServices.Animal.Api.Data;
using MicroServices.Animal.Api.Data.Domains.Device;
using MicroServices.Animal.Api.Data.Mementos;
using MicroServices.Animal.Api.Data.Projections;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Commands;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Responses.Commands;

namespace MicroServices.Animal.Api.Features.Animal.Cqrs.Handlers.Commands
{
	public class CreateAnimalCommandHandler : IRequestHandler<CreateAnimalCommand,
		Either<ExceptionResponse, CreateAnimalCommandResponse>>
	{
		private readonly IDesignTimeDbContextFactory<AnimalContext> _animalContextFactory;

		public CreateAnimalCommandHandler(IDesignTimeDbContextFactory<AnimalContext> animalContextFactory)
		{
			_animalContextFactory = animalContextFactory;
		}

		public async Task<Either<ExceptionResponse, CreateAnimalCommandResponse>> Handle(CreateAnimalCommand request,
			CancellationToken cancellationToken)
		{
			var commandParams = request.AnimalPayload;

			DeviceAssignedToAnimal deviceDto;
			Data.Domains.Animal.Animal animal = null;

			using (var context = _animalContextFactory.CreateDbContext(new string[0]))
			{
				deviceDto = await GetDeviceByCompositeKey(context, commandParams.DeviceIdentifier)
					.Match(some => some,
						() => new DeviceAssignedToAnimal());

				// todo MF: needs to be refactored
				if (deviceDto.DeviceID == 0)
					return ExceptionResponse.With(ErrorMessage: "Device not found.", HttpStatusCode: HttpStatusCode.NotFound);

				if (deviceDto.IssueToPropertyIdentifierID > 0)
					if (!deviceDto.AnimalAssignedToDevice)
					{
						animal = AssignNewAnimalToDevice(context, deviceDto, commandParams.CurrentPropertyIdentifierId,
							commandParams.TransactionDate, request.RequestId);

						await ApplyPropertyRules(context, animal, deviceDto.AssignedToPropertyIdentifierID, commandParams.TransactionDate,
							request.RequestId);
						await context.SaveChangesAsync(request.RequestId);
					}
					else
					{
						animal = deviceDto.Animal;
						return new CreateAnimalCommandResponse(animal);
					}
			}

			return new CreateAnimalCommandResponse(animal);
		}


		private async Task ApplyPropertyRules(AnimalContext context, Data.Domains.Animal.Animal animal,
			long assignedToPropertyId,
			DateTime activationDate, long? requestId)
		{
			var activeRules = await context.PropertyAnimalStatusRules.Include(x => x.ProgramStatus)
				.Include(x => x.ProgramStatus.Program)
				.Where(r => r.PropertyIdentifierID == assignedToPropertyId &&
				            (r.RuleInactivationDate == null || r.RuleInactivationDate > DateTime.Now) &&
				            r.RuleActivationDate <= DateTime.Now)
				.ToArrayAsync();

		}

		private Data.Domains.Animal.Animal AssignNewAnimalToDevice(AnimalContext context, DeviceAssignedToAnimal deviceDto,
			int? currentPropertyId, DateTime? latestTransferDate,
			long? requestId)
		{
			var animal = context.Animals.Add(new AnimalMemento(0, DateTime.Now, deviceDto.Species, currentPropertyId ?? deviceDto.AssignedToPropertyIdentifierID, requestId ?? 0));

			var deviceAssignment = new DeviceAssignment
			{
				DeviceID = deviceDto.DeviceID,
				AssignmentDate = deviceDto.AssignmentDate ?? DateTime.Now,
				CreatedRequestID = requestId ?? 0
			};
			context.DeviceAssignments.Add(deviceAssignment);
			// todo: refactor after finishing memento
			//animal.Entity.DeviceAssignment.Add(deviceAssignment);

			return Data.Domains.Animal.Animal.FromMemento(animal.Entity);
		}

		private async Task<Option<DeviceAssignedToAnimal>> GetDeviceByCompositeKey(AnimalContext context,
			string deviceCompositekey)
		{
			var device = await context.Devices.AsNoTracking()
				.Include(x => x.DeviceDefinition)
				.Include(x => x.DeviceAssignment).ThenInclude(p => p.Animal)
				.Where(da => da.NLISID == deviceCompositekey || da.RFID == deviceCompositekey)
				.FirstOrDefaultAsync();

			if (device == null)
				return Option<DeviceAssignedToAnimal>.None;

			Data.Domains.Animal.Animal animal = null;
			if (device.DeviceAssignment.Any())
				animal = device.DeviceAssignment.FirstOrDefault(devAssgn => devAssgn.ReplacementDate == null)
					?.Animal; //TODO:MF this criteria should be considered				

			var deviceAssignedToAnimal = new DeviceAssignedToAnimal
			{
				Species = device.DeviceDefinition?.SpeciesID,
				IsPostBreederDevice = device.DeviceDefinition != null ? device.DeviceDefinition.IsPostBreederDevice : false,
				Animal = animal,
				DeviceAssignment = device.DeviceAssignment.FirstOrDefault(p => p.ReplacementDate == null),
				NLISID = device.NLISID,
				RFID = device.RFID,
				AssignedToPropertyIdentifierID = device.AssignedToPropertyIdentifierID,
				AssignmentDate = device.AssignmentDate,
				DeviceID = device.DeviceID,
				ExcludedDate = device.ExcludedDate,
				ExcludedReasonID = device.ExcludedReasonID,
				IssueToPropertyIdentifierID = device.IssueToPropertyIdentifierID
			};
			return new Some<DeviceAssignedToAnimal>(deviceAssignedToAnimal);
		}
	}
}