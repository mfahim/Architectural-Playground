using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LanguageExt;
using MediatR;
using MicroServices.Animal.Api.Data.Domains.Device;
using MicroServices.Animal.Api.Data.Factories;
using MicroServices.Animal.Api.Data.Projections;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Commands;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Queries;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Responses.Queries;

namespace MicroServices.Animal.Api.Features.Animal.Cqrs.Handlers.Queries
{
	public class GetAnimalByAnimalIdQueryHandler : IRequestHandler<GetAnimalByAnimalIdQuery,
		Either<ExceptionResponse, GetAnimalByDeviceIdentifierQueryResponse>>
	{
		private readonly IDapperConnectionFactory _dapperConnection;

		public GetAnimalByAnimalIdQueryHandler(IDapperConnectionFactory dapperConnection)
		{
			_dapperConnection = dapperConnection;
		}

		public async Task<Either<ExceptionResponse, GetAnimalByDeviceIdentifierQueryResponse>> Handle(
			GetAnimalByAnimalIdQuery queryParams, CancellationToken cancellationToken)
		{
			GetAnimalByDeviceIdentifierQueryResponse response;

			using (var connection = _dapperConnection.GetOpenConnection())
			{
				var query = "select animal.*, da.*, device.* from Animal.Animal animal " +
				            "inner join Animal.DeviceAssignment da on da.AnimalID = animal.AnimalId " +
				            "inner join Device.Device device on device.DeviceID = da.DeviceID " +
				            $"where da.ReplacementDate is null and animal.AnimalId = {queryParams.AnimalId}";

				var currentAnimal = await connection
					.QueryAsync<Data.Domains.Animal.Animal, DeviceAssignment, Device, DeviceAssignedToAnimal>(query,
						(animal, deviceAssignment, device) =>
						{
							if (animal == null)
								return null;

							return new DeviceAssignedToAnimal
							{
								AnimalId = animal.AnimalId,
								DeviceID = device.DeviceID,
								CurrentPropertyIdentifierID = animal.CurrentPropertyIdentifierID,
								CreatedRequestID = animal.CreatedRequestID,
								LastModifiedRequestID = animal.LastModifiedRequestID,
								BirthDate = animal.BirthDate,
								DeviceAssignment = deviceAssignment,
								NLISID = device.NLISID,
								RFID = device.RFID,
								ExcludedDate = animal.ExcludedDate,
								ExcludedReasonID = animal.ExcludedReasonID,
								OriginPropertyIdentifierID = animal.OriginPropertyIdentifierID,
								OriginDate = animal.OriginDate,
								AssignmentDate = deviceAssignment.AssignmentDate,
								AssignedToPropertyIdentifierID = device.AssignedToPropertyIdentifierID,
								IssueToPropertyIdentifierID = device.AssignedToPropertyIdentifierID,
								Species = animal.SpeciesID
							};
						}, splitOn: "AnimalId, DeviceId");

				if (!currentAnimal.Any())
					return ExceptionResponse.With(
						ErrorMessage: $"Associated Animal's data is not found. AnimalId = {queryParams.AnimalId}",
						HttpStatusCode: HttpStatusCode.NotFound);

				response = new GetAnimalByDeviceIdentifierQueryResponse(currentAnimal.FirstOrDefault());
			}
			return response;
		}
	}
}