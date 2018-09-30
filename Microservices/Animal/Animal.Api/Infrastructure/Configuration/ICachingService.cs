using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MicroServices.Animal.Api.Infrastructure.AuofacModule;
using ValidationOutcome = MicroServices.Animal.Api.Common.Cqrs.ValidationOutcome;

namespace MicroServices.Animal.Api.Infrastructure.Configuration
{
	public interface ICachingService : IDisposable
	{
		Task<T> Get<T>(string key, TimeSpan? expireIn = null);

		Task<T> Get<T>(string key, Func<Task<T>> method);

		Task<T> Get<T>(string key, TimeSpan expireIn, Func<Task<T>> method);

		void Clear(string key);
	}
	public interface IConfigurationSetting
	{
		string ApplicationName { get; }
		string ConsulIpAddress { get; }
	}

	public interface IHttpClient
	{
		Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer");

		Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");

		Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");

		Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");

		Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, string authorizationToken = null,
			string requestId = null, string authorizationMethod = "Bearer");
	}

	public interface IRestResultFactory
	{
		Task<RestResult> Combine<T>(RestResult existingResult, Func<Task<T>> action, HttpStatusCode succesStatusCode = HttpStatusCode.OK);

		RestResult Build(Exception ex);

		Task<RestResult> BuildGeneric(HttpStatusCode statusCode, string message = "", string description = "");

	}

	public class RestResultFactory : IRestResultFactory
	{
		private readonly ILogger _logger;

		public RestResultFactory(ILogger logger)
		{
			this._logger = logger;
		}

		public async Task<RestResult> Combine<T>(RestResult existingRestResult, Func<Task<T>> action, HttpStatusCode succesStatusCode = HttpStatusCode.OK)
		{
			try
			{
				var actionResult = action == null ? default(T) : await action();

				var restActionResult = BuildRestResult(actionResult, succesStatusCode);

				//var aggregatedResult = AggregatedResult(existingRestResult, restActionResult);

				//return aggregatedResult;
				return restActionResult;
			}
			catch (Exception ex)
			{
				await _logger.Error(null, ex);

				return Build(ex);
			}
		}

		public RestResult Build(Exception ex)
		{
			HttpStatusCode statusCode;

			return new RestResult(ex.Message, HttpStatusCode.RequestTimeout);
		}

		private static RestResult BuildRestResult(object actionResult, HttpStatusCode succesStatusCode)
		{
			var statusCode = actionResult == null ? HttpStatusCode.NoContent : succesStatusCode;

			return actionResult as RestResult
				   //?? new RestResult(RestContent.CreateWithSuccess(actionResult), statusCode);
					?? new RestResult(actionResult, statusCode);
		}

		private static IEnumerable<ValidationOutcome> AggregateOutcomes(IEnumerable<ValidationOutcome> sourceList, IEnumerable<ValidationOutcome> destList)
		{
			if (destList == null)
				return sourceList;

			return sourceList == null
				? destList
				: destList.Union(sourceList).GroupBy(g => new { g.Code, g.Message }).Select(g => g.First()).ToList();
		}

		public async Task<RestResult> BuildGeneric(HttpStatusCode statusCode, string message = "", string description = "")
		{
			if (statusCode == HttpStatusCode.OK && message == "")
			{
				return new RestResult((object)null);
			}

			var restContent = RestContent.CreateWithErrors(new[]
			{
				new ValidationOutcome
				{
					OutcomeStatus = "Error",
					Code = "Generic",
					Message = message,
					Description = description
				}
			});

			return new RestResult(restContent, statusCode);
		}
	}

	public class RestContent
	{
		public object Value { get; }

		public IEnumerable<ValidationOutcome> Errors { get; }
		public IEnumerable<ValidationOutcome> Infos { get; }
		public IEnumerable<ValidationOutcome> Warnings { get; }

		public RestContent(object value = null, IEnumerable<ValidationOutcome> errors = null,
			IEnumerable<ValidationOutcome> warnings = null,
			IEnumerable<ValidationOutcome> infos = null)
		{
			Value = value;
			Errors = errors;
			Warnings = warnings;
			Infos = infos;
		}

		public static RestContent CreateWithSuccess(object value) => new RestContent(value: value);

		public static RestContent CreateWithErrors(IEnumerable<ValidationOutcome> errors,
			IEnumerable<ValidationOutcome> warnings = null,
			IEnumerable<ValidationOutcome> infos = null) =>
			new RestContent(errors: errors, warnings: warnings, infos: infos);

		public static RestContent CreateWithWarnings(object value, IEnumerable<ValidationOutcome> warnings) =>
			new RestContent(value: value, warnings: warnings);

		public static RestContent CreateWithGenericError(Exception exception) => new RestContent(errors: GenericError(exception));
		public static RestContent CreateWithGenericError(string message) => new RestContent(errors: GenericError(new Exception(message)));

		public static ValidationOutcome[] GenericError(Exception exception) =>
			new[] {
				new ValidationOutcome
				{
					Code = "Generic",
					OutcomeStatus = "Error",
					Message = exception.Message,
#if DEBUG
					InnerException = exception.InnerException?.Message,
					StackTrace = exception.StackTrace
#endif
				}};
	}
}