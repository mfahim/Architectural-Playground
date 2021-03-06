using System;
using System.Net;
using MicroServices.Animal.Api.Infrastructure.Configuration;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;

namespace MicroServices.Animal.Api.Common.Exceptions
{
	public class NlisInternalServerException : ApplicationException
	{
		public static RestContent CreateRestContent(string message)
		{
			return RestContent.CreateWithGenericError(message);
		}
		public static HttpStatusCode HttpStatus() => HttpStatusCode.InternalServerError;
	}
}