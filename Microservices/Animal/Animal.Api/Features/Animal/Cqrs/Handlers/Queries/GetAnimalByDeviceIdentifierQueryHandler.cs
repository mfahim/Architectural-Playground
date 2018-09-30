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
using MicroServices.Animal.Api.Data.Projections;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Commands;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Queries;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Responses.Queries;

namespace MicroServices.Animal.Api.Features.Animal.Cqrs.Handlers.Queries
{
	public class GetAnimalByDeviceIdentifierQueryHandler : IRequestHandler<GetAnimalByDeviceIdentifierQuery,
		Either<ExceptionResponse, GetAnimalByDeviceIdentifierQueryResponse>>
	{
		private readonly IDesignTimeDbContextFactory<AnimalContext> _animalContextFactory;

		public GetAnimalByDeviceIdentifierQueryHandler(IDesignTimeDbContextFactory<AnimalContext> animalContextFactory)
		{
			_animalContextFactory = animalContextFactory;
		}

		public async Task<Either<ExceptionResponse, GetAnimalByDeviceIdentifierQueryResponse>> Handle(
			GetAnimalByDeviceIdentifierQuery request, CancellationToken cancellationToken)
		{
			GetAnimalByDeviceIdentifierQueryResponse response;

			using (var context = _animalContextFactory.CreateDbContext(new string[0]))
			{
				IQueryable<Device> deviceQuery;
				if (!string.IsNullOrEmpty(request.NlisId))
					deviceQuery = context.Devices
						.Include(x => x.DeviceDefinition)
						.Include(x => x.DeviceAssignment).ThenInclude(p => p.Animal)
						.Where(da => da.NLISID == request.NlisId);
				else
					deviceQuery = context.Devices
						.Include(x => x.DeviceDefinition)
						.Include(x => x.DeviceAssignment).ThenInclude(p => p.Animal)
						.Where(da => da.RFID == request.RfId);

				var entity = await deviceQuery.FirstOrDefaultAsync();

				Data.Domains.Animal.Animal animal = null;
				if (entity != null && entity.DeviceAssignment.Any())
					animal = entity.DeviceAssignment.FirstOrDefault(devAssgn => devAssgn.ReplacementDate == null)?.Animal;
				else
					return ExceptionResponse.With(
						ErrorMessage: $"Associated Animal's data is not found. AnimalId = {request.NlisId ?? request.RfId}",
						HttpStatusCode: HttpStatusCode.NotFound);

				var deviceAssignedToAnimal = new DeviceAssignedToAnimal
				{
					Species = entity.DeviceDefinition.SpeciesID,
					IsPostBreederDevice = entity.DeviceDefinition.IsPostBreederDevice,
					NLISID = entity.NLISID,
					RFID = entity.RFID,
					AssignedToPropertyIdentifierID = entity.AssignedToPropertyIdentifierID,
					AssignmentDate = entity.AssignmentDate,
					DeviceID = entity.DeviceID,
					ExcludedDate = entity.ExcludedDate,
					ExcludedReasonID = entity.ExcludedReasonID,
					IssueToPropertyIdentifierID = entity.IssueToPropertyIdentifierID,
					CreatedRequestID = entity.CreatedRequestID,
					LastModifiedRequestID = entity.LastModifiedRequestID
				};

				response = new GetAnimalByDeviceIdentifierQueryResponse(deviceAssignedToAnimal);
			}

			return response;
		}
	}
}