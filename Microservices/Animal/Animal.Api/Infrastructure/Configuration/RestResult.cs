using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using MicroServices.Animal.Api.Common.Cqrs;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MicroServices.Animal.Api.Infrastructure.Configuration
{
	public class RestResult
	{
		public HttpStatusCode HttpStatusCode { get; set; }
		public RestContent RestContent { get; set; }

		public bool HasErrors => HttpStatusCode > HttpStatusCode.BadRequest || (RestContent?.Errors?.Any() ?? false);

		public RestResult(object value, HttpStatusCode statusCode)
		{
			this.HttpStatusCode = statusCode;
			this.RestContent = RestContent.CreateWithSuccess(value);
		}
		public RestResult(object value)
		{
			this.HttpStatusCode = HttpStatusCode.OK;
			this.RestContent = RestContent.CreateWithSuccess(value);
		}
		public RestResult(HttpResponseMessage response)
		{
			this.HttpStatusCode = response.StatusCode;
			this.RestContent = Task.Run(() => BuildRestContent(response.Content)).GetAwaiter().GetResult();
		}
		public RestResult(RestContent restContent, HttpStatusCode statusCode)
		{
			this.RestContent = restContent;
			this.HttpStatusCode = statusCode;
		}

		public virtual T ToObject<T>() where T : class
		{
			return RestContent?.Value is JToken
				? JsonConvert.DeserializeObject<T>(((JToken)RestContent.Value).ToString())
				: null;
		}

		private static async Task<RestContent> BuildRestContent(HttpContent content)
		{
			var valueStr = await content.ReadAsStringAsync();

			if (!valueStr.Trim().StartsWith("{") && valueStr.Trim().StartsWith("["))
				return RestContent.CreateWithSuccess(valueStr);

			try
			{
				var jvalue = JToken.Parse(valueStr);

				var dic = new[] { "Value", "Errors", "Infos", "Warnings" }.ToDictionary(x => x, x => jvalue.SelectToken(x));
				var value = dic["Value"]?.Value<JToken>();
				var errors = dic["Errors"]?.Select(x => x.ToObject<ValidationOutcome>()).ToList();
				var warnings = dic["Warnings"]?.Select(x => x.ToObject<ValidationOutcome>()).ToList();
				var infos = dic["Infos"]?.Select(x => x.ToObject<ValidationOutcome>()).ToList();


				if (errors != null)
					return RestContent.CreateWithErrors(errors, warnings, infos);
				if (warnings != null)
					return RestContent.CreateWithWarnings(value, warnings);

				return RestContent.CreateWithSuccess(value);
			}
			catch (Exception ex)
			{
				return RestContent.CreateWithGenericError(ex);
			}
		}
	}

	public static class HttpRequestMessageExtentions
	{
		public static string GetInternalRequestId1(this HttpRequest request)
		{
			request.Headers.TryGetValue(HttpHeaders.InternalRequestIdHeader, out StringValues internalRequestId);
			return internalRequestId;
		}

		public static long? GetRequestId1(this HttpRequest request)
		{
			request.Headers.TryGetValue(HttpHeaders.RequestIdHeader, out StringValues requestId);
			return string.IsNullOrEmpty(requestId) ? 50000000 : long.Parse(requestId);
		}

		public static string GetAccountId(this HttpRequest request)
		{
			request.Headers.TryGetValue(HttpHeaders.AccountId, out StringValues accountId);
			return accountId;
		}


		public static string GetUserId(this HttpRequest request)
		{
			request.Headers.TryGetValue(HttpHeaders.UserIdHeader, out StringValues userId);
			return userId;
		}

		public static string GetClientId(this HttpRequest request)
		{

			request.Headers.TryGetValue(HttpHeaders.ClientIdHeader, out StringValues clientId);
			return clientId;
		}

		public static string GetReferenceEndpoint(this HttpRequest request)
		{
			request.Headers.TryGetValue(HttpHeaders.EndpointHeader, out StringValues referenceEndpointId);
			return referenceEndpointId;
		}

		public static string GetReferenceEndpointVersion(this HttpRequest request)
		{
			request.Headers.TryGetValue(HttpHeaders.EndpointVersionHeader, out StringValues endpointVersion);
			return endpointVersion;
		}

		public static short? GetReferenceEndpointId(this HttpRequest request)
		{
			request.Headers.TryGetValue(HttpHeaders.EndpointIdHeader, out StringValues endpointId);
			return string.IsNullOrEmpty(endpointId) ? null : (short?)short.Parse(endpointId);
		}

		public static bool AsyncPreferred(this HttpRequestMessage request)
		{
			if (!request.Headers.Contains("Prefer")) return false;
			return request.Headers.GetValues("Prefer")
				.Single()
				.Split(',')
				.Select(p => p.Trim().ToLower())
				.ToList()
				.Contains("respond-async");
		}


		public static void SetHeader(this HttpRequest request, string headerName, string headerValue)
		{
			if (request.Headers.ContainsKey(headerName))
				request.Headers.Remove(headerName);
			request.Headers.Add(headerName, headerValue);
		}

		public static string GetEndpoint(this HttpRequest request)
		{
			request.Headers.TryGetValue(HttpHeaders.EndpointHeader, out StringValues referenceEndpointId);
			return referenceEndpointId;
		}

		public static Uri GetRequestUri(this HttpRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));
			if (string.IsNullOrWhiteSpace(request.Scheme))
				throw new ArgumentException("Http request Scheme is not specified");
			if (!request.Host.HasValue)
				throw new ArgumentException("Http request Host is not specified");
			var stringBuilder = new StringBuilder();
			stringBuilder.Append(request.Scheme).Append("://").Append(request.Host);
			if (request.Path.HasValue)
				stringBuilder.Append(request.Path.Value);
			if (request.QueryString.HasValue)
				stringBuilder.Append(request.QueryString);
			return new Uri(stringBuilder.ToString());
		}

		public static string GetRequestId(this HttpResponse response)
		{
			response.Headers.TryGetValue(HttpHeaders.RequestIdHeader, out StringValues requestId);
			return requestId;
		}
	}
	public static class HttpHeaders
	{
		public const string RequestIdHeader = "requestId";
		public const string InternalRequestIdHeader = "X-InternalRequestId";
		public const string UserIdHeader = "X-UserId";
		public const string ClientIdHeader = "X-ClientId";
		public static string EndpointHeader = "X-Endpoint";
		public static string AccountId = "X-AccountId";
		public static string EndpointVersionHeader = "X-EndpointVersion";
		public static string EndpointIdHeader = "X-EndpointId";
	}
}