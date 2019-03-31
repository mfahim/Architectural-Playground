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
using MicroServices.Animal.Api.Data.Domains.Animal;
using MicroServices.Animal.Api.Data.Projections;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Commands;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Responses.Commands;
using MicroServices.Animal.Api.Infrastructure.Configuration;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;

namespace MicroServices.Animal.Api.Features.Animal.Cqrs.Handlers.Commands
{
	public class UpdateAnimalCommandHandler : IRequestHandler<UpdateAnimalCommand,
		Either<ExceptionResponse, UpdateAnimalCommandResponse>>
	{
		private readonly IDesignTimeDbContextFactory<AnimalContext> _animalContextFactory;
		private readonly ICachingService _cachingService;

		public UpdateAnimalCommandHandler(IDesignTimeDbContextFactory<AnimalContext> animalContextFactory,
			ICachingService cachingService)
		{
			_animalContextFactory = animalContextFactory;
			_cachingService = cachingService;
		}

		public async Task<Either<ExceptionResponse, UpdateAnimalCommandResponse>> Handle(UpdateAnimalCommand request,
			CancellationToken cancellationToken)
		{
			var commandParams = request.AnimalPayload;

			DeviceAssignedToAnimal deviceDto;
			Data.Domains.Animal.Animal existingAnimal = null;
			using (var context = _animalContextFactory.CreateDbContext(new string[0]))
			{
				deviceDto = await GetDeviceByCompositeKey(context, commandParams.DeviceIdentifier)
					.Match(some => some, () => throw new InvalidOperationException("Device not found."));

				existingAnimal = deviceDto.Animal;
				if (existingAnimal == null)
					return ExceptionResponse.With(
						ErrorMessage: $"animal/{request.AnimalPayload.DeviceCompositeKey}",
						HttpStatusCode: HttpStatusCode.NotFound);

				existingAnimal.LastModifiedRequestID = request.RequestId;

				await context.SaveChangesAsync(request.RequestId);
			}
			return new UpdateAnimalCommandResponse(existingAnimal);
		}

		public async Task<AnimalExcludedReason> GetExcludedReasonByCode(AnimalContext animalContext, string exclusionReason)
		{
			var excludedReason = await _cachingService.Get($"exclusion-{exclusionReason}",
				TimeSpan.FromHours(24), () =>
				{
					return animalContext.Set<AnimalExcludedReason>()
						.FirstOrDefaultAsync(x => x.Description.Equals(exclusionReason, StringComparison.InvariantCultureIgnoreCase));
				});

			if (excludedReason == null)
				throw new BadRequestException($"Invalid exclusion reason code '{exclusionReason}' provided");

			return excludedReason;
		}

		private async Task<Option<DeviceAssignedToAnimal>> GetDeviceByCompositeKey(AnimalContext context,
			string deviceCompositekey)
		{
			var device = await context.Devices
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

			var deviceAssignmentValue = new DeviceAssignedToAnimal
			{
				Species = device.DeviceDefinition.SpeciesID,
				IsPostBreederDevice = device.DeviceDefinition.IsPostBreederDevice,
				Animal = animal,
				DeviceAssignment = device.DeviceAssignment.FirstOrDefault(da => da.ReplacementDate == null),
				NLISID = device.NLISID,
				RFID = device.RFID,
				AssignedToPropertyIdentifierID = device.AssignedToPropertyIdentifierID,
				AssignmentDate = device.AssignmentDate,
				DeviceID = device.DeviceID,
				ExcludedDate = device.ExcludedDate,
				ExcludedReasonID = device.ExcludedReasonID,
				IssueToPropertyIdentifierID = device.IssueToPropertyIdentifierID
			};
			return Option<DeviceAssignedToAnimal>.Some(deviceAssignmentValue);
		}
	}
}