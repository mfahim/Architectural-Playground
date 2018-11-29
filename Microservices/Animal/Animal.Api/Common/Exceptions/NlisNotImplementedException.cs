using System;
using System.Net;
using MicroServices.Animal.Api.Infrastructure.Configuration;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;

namespace MicroServices.Animal.Api.Common.Exceptions
{
	public class NlisNotImplementedException : ApplicationException
	{
		public static RestContent CreateRestContent()
		{
			return RestContent.CreateWithGenericError("Not Implemented Exception");
		}
		public static HttpStatusCode HttpStatus() => HttpStatusCode.NotImplemented;
	}
}