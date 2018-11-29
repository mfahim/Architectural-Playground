using System;
using System.Net;
using MicroServices.Animal.Api.Infrastructure.Configuration;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;

namespace MicroServices.Animal.Api.Common.Exceptions
{
	public class ForbiddenException : ApplicationException
	{
		public static RestContent CreateRestContent()
		{
			return RestContent.CreateWithGenericError("Unauthorized Access");
		}
		public static HttpStatusCode HttpStatus() => HttpStatusCode.Forbidden;
	}
}