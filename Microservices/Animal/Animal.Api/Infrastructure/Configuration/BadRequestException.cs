using System;
using System.Net;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;

namespace MicroServices.Animal.Api.Infrastructure.Configuration
{
	public class BadRequestException : ApplicationException
	{
		public BadRequestException(string errorContent) : base(errorContent)
		{ }

		public static RestContent CreateRestContent(string errorContent)
		{
			if (string.IsNullOrEmpty(errorContent))
				return RestContent.CreateWithGenericError("Bad Request");

			return RestContent.CreateWithGenericError(errorContent);
		}
		public static HttpStatusCode HttpStatus() => HttpStatusCode.BadRequest;
	}
}