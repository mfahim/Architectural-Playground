using LanguageExt;
using MediatR;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Commands;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Responses.Queries;

namespace MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Queries
{
	public class GetAnimalByDeviceIdentifierQuery : BaseQuery,
		IRequest<Either<ExceptionResponse, GetAnimalByDeviceIdentifierQueryResponse>>
	{
		public string NlisId { get; set; }

		public string RfId { get; set; }
	}
}