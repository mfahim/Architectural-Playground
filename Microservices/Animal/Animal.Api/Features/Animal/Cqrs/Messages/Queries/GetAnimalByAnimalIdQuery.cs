using LanguageExt;
using MediatR;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Commands;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Responses.Queries;

namespace MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Queries
{
	public class GetAnimalByAnimalIdQuery : BaseQuery,
		IRequest<Either<ExceptionResponse, GetAnimalByDeviceIdentifierQueryResponse>>
	{
		public long AnimalId { get; set; }
	}

	public class BaseQuery
	{
		public long? RequestId { get; set; }
		public long AccountId { get; set; }
	}
}