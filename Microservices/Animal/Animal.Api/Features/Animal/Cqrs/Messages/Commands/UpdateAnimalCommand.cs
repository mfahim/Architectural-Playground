using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using MicroServices.Animal.Api.Common.Cqrs;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Responses.Commands;
using MicroServices.Animal.Api.Infrastructure.Configuration;

namespace MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Commands
{
	public class UpdateAnimalCommand : BaseCommand, IRequest<Either<ExceptionResponse, UpdateAnimalCommandResponse>>
	{
		public UpdateAnimalCommand(AnimalPayload animalPayload, long? requestId, long accountId, string clientId)
			: base(requestId, accountId, clientId)
		{
			AnimalPayload = animalPayload;
		}

		public AnimalPayload AnimalPayload { get; }

		public static UpdateAnimalCommand Create
			(AnimalPayload killAnimalRequest, HttpRequest httpRequest)
		{
			int.TryParse(httpRequest.GetAccountId(), out int accountId);
			// todo : test account for testing only, needs to be removed after all settled
			if (accountId == 0)
				accountId = 137;

			return new UpdateAnimalCommand(killAnimalRequest, httpRequest.GetRequestId1(), accountId, httpRequest.GetClientId());
		}
	}
}