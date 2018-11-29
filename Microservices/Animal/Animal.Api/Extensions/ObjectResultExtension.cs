using System.Net;
using Microsoft.AspNetCore.Mvc;
using MicroServices.Animal.Api.Infrastructure.Configuration;

namespace MicroServices.Animal.Api.Extensions
{
	public static class ObjectResultExtension
	{
		public static ObjectResult CreateResponse(RestResult restResult)
		{
			if (!restResult.HasErrors && restResult.HttpStatusCode == HttpStatusCode.Created)
				return new ObjectResult(restResult.RestContent) {StatusCode = (int?) HttpStatusCode.Created};

			if (!restResult.HasErrors)
				return new OkObjectResult(restResult.RestContent);

			return new ObjectResult(restResult.RestContent) {StatusCode = (int?) restResult.HttpStatusCode};
		}
	}
}