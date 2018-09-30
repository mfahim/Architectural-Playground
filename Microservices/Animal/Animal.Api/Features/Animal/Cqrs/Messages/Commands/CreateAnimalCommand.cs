using System;
using System.Net;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using MicroServices.Animal.Api.Common.Cqrs;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Responses.Commands;
using MicroServices.Animal.Api.Infrastructure.Configuration;

namespace MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Commands
{
	public class CreateAnimalCommand : BaseCommand, IRequest<Either<ExceptionResponse, CreateAnimalCommandResponse>>
	{
		public CreateAnimalCommand(AnimalPayload animalPayload, long? requestId, long accountId, string clientId)
			: base(requestId, accountId, clientId)
		{
			AnimalPayload = animalPayload;
		}

		public AnimalPayload AnimalPayload { get; }

		public static CreateAnimalCommand Create
			(AnimalPayload killAnimalRequest, HttpRequest httpRequest)
		{
			int.TryParse(httpRequest.GetAccountId(), out int accountId);
			// todo : test account for testing only, needs to be removed after all settled
			if (accountId == 0)
				accountId = 137;

			return new CreateAnimalCommand(killAnimalRequest, httpRequest.GetRequestId1(), accountId, httpRequest.GetClientId());
		}

		public static CreateAnimalCommand Create
			(AnimalPayload killAnimalRequest, CreateAnimalNotification notification)
		{
			return new CreateAnimalCommand(killAnimalRequest, notification.RequestId, notification.AccountId,
				notification.ClientId);
		}
	}

	public class CreateAnimalNotification : BaseNotification, INotification
	{
		public long? AnimalId { get; set; }
		public DateTime TransactionDate { get; set; }

		public int? CurrentPropertyIdentifierId { get; set; }

		// either nlisId or rfid
		public string DeviceIdentifier { get; set; }

		public string ExclusionReason { get; set; }
	}

	public class BaseNotification
	{
		public long? RequestId { get; set; }
		public long AccountId { get; set; }

		// the client id from auth token
		public string ClientId { get; set; }
	}

	public class ExceptionResponse
	{
		public Exception InnerException { get; }
		public HttpStatusCode HttpStatusCode { get; }
		public string ErrorMessage { get; }

		public ExceptionResponse(Exception InnerException = null, HttpStatusCode HttpStatusCode = HttpStatusCode.InternalServerError, string ErrorMessage = null)
		{
			this.InnerException = InnerException;
			this.HttpStatusCode = HttpStatusCode;
			this.ErrorMessage = ErrorMessage;
		}

		public static ExceptionResponse With(Exception InnerException = null, HttpStatusCode HttpStatusCode = HttpStatusCode.InternalServerError, string ErrorMessage = null)
		{
			return new ExceptionResponse(InnerException: InnerException,
				HttpStatusCode: HttpStatusCode,
				ErrorMessage: ErrorMessage ?? InnerException?.Message);
		}
	}
}