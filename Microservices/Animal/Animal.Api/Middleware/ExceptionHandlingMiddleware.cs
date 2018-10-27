using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Rest;
using MicroServices.Animal.Api.Common.Cqrs;
using MicroServices.Animal.Api.Common.Exceptions;
using MicroServices.Animal.Api.Infrastructure.AuofacModule;
using MicroServices.Animal.Api.Infrastructure.Configuration;
using Newtonsoft.Json;
using Serilog.Events;

namespace MicroServices.Animal.Api.Middleware
{
	/// <summary>
	///     this contains the logic for global exception handling
	/// </summary>
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly JsonSerializerSettings _serializerSettings;

		public ExceptionHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
			_serializerSettings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
		}

		public async Task Invoke(HttpContext context, ILogger logger, IConfigurationSetting setting)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await logger.Error(new ApplicationLogMessage(setting, context.Request.GetRequestId1()?.ToString(),
					context.Request.GetInternalRequestId1(),
					string.Empty, LogEventLevel.Error, ex));
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			RestContent restContent = null;
			HttpStatusCode statusCode;
			if (exception is UnauthorizedAccessException)
			{
				restContent = ForbiddenException.CreateRestContent();
				statusCode = ForbiddenException.HttpStatus();
			}
			else if (exception is ValidationException)
			{
				var ex = (ValidationException) exception;
				var ruleResults = ex.Details as RuleResultModel[];
				restContent = NlisValidationException.CreateRestContent(ruleResults);
				statusCode = NlisValidationException.HttpStatus();
			}
			else if (exception is NotImplementedException)
			{
				restContent = NlisNotImplementedException.CreateRestContent();
				statusCode = NlisNotImplementedException.HttpStatus();
			}
			else
			{
				restContent = NlisInternalServerException.CreateRestContent(exception.Message);
				statusCode = NlisInternalServerException.HttpStatus();
			}

			context.Response.StatusCode = (int) statusCode;
			// the microservice content type is set to json for exception handling
			context.Response.ContentType = "application/json";
			var jsonResponse = JsonConvert.SerializeObject(restContent, _serializerSettings);
			await context.Response.WriteAsync(jsonResponse);
		}
	}
}