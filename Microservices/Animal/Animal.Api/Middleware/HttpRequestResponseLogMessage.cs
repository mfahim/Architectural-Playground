using System;
using System.Diagnostics;
using System.Dynamic;
using Microsoft.AspNetCore.Http;
using MicroServices.Animal.Api.Infrastructure.Configuration;

namespace MicroServices.Animal.Api.Middleware
{
	public class HttpRequestResponseLogMessage : ILogMessage
	{
		private readonly IConfigurationSetting _configurationSetting;
		private static string LargePayloadMessage = "Large payload";
		private const int MaximumPayloadLimitInBytes = 100000;

		public HttpRequestResponseLogMessage(IConfigurationSetting configurationSetting,
			HttpRequest request, HttpResponse response, Stopwatch stopwatch, string requestContent, string responseContent)
		{
			_configurationSetting = configurationSetting;
			var requestDetails = BuildRequestDetails(request, requestContent);
			var responseDetails = BuildResponseDetails(response, request, stopwatch, responseContent);

			RequestId = requestDetails.RequestId;
			InternalRequestId = requestDetails.InternalRequestId;
			Request = requestDetails;
			UserId = requestDetails.UserId;
			ClientId = requestDetails.ClientId;
			RequestUri = requestDetails.RequestUri;
			ClientIpAddress = requestDetails.ClientIpAddress;
			ProcessingTime = responseDetails.TotalMilliseconds;
			Response = responseDetails;
			Endpoint = request?.GetEndpoint();
		}


		/// <summary>
		/// name of application e.g. refresh API v1
		/// </summary>
		public string ApplicationName => _configurationSetting.ApplicationName;

		public string LogEventLevel => "Information";
		public string RequestId { get; }

		public string InternalRequestId { get; }
		public string MachineName => Environment.MachineName;
		public object Request { get; }
		public object Response { get; }

		public string Message => null;

		public string UserId { get; }

		public string ClientId { get; }

		public string ClientIpAddress { get; }
		public DateTime Time => DateTime.UtcNow;
		public double? ProcessingTime { get; }
		public string RequestUri { get; }

		public string Exception => null;
		public string Endpoint { get; }


		private dynamic BuildRequestDetails(HttpRequest request, string requestContent)
		{
			if (request == null)
				return null;

			dynamic details = new ExpandoObject();
			details.Method = request.Method;
			details.RequestUri = request.GetRequestUri().ToString();
			details.ClientIpAddress = request.HttpContext.Connection.RemoteIpAddress.ToString();
			if (request.Headers != null)
			{
				details.RequestId = request.GetRequestId1().ToString();
				details.InternalRequestId = request.GetInternalRequestId1();
				details.UserId = request.GetUserId();
				details.ClientId = request.GetClientId();
				details.Headers = request.Headers;
			}

			if (request.ContentLength.HasValue)
			{
				var headersContentLength = request.ContentLength.Value;

				details.Content = new
				{
					Length = headersContentLength,
					Content = requestContent
				};
			}

			return details;
		}
		private static dynamic BuildResponseDetails(HttpResponse response, HttpRequest request, Stopwatch stopwatch, string responseContent)
		{
			if (response == null)
				return new { stopwatch.Elapsed.TotalMilliseconds };


			dynamic details = new ExpandoObject();
			details.StatusCode = response.StatusCode;

			if (!string.IsNullOrEmpty(responseContent))
			{
				var headersContentLength = response.ContentLength;

				if (string.Equals(request.Method, System.Net.WebRequestMethods.Http.Get, StringComparison.OrdinalIgnoreCase) &&
				    ((headersContentLength.HasValue && headersContentLength.Value > MaximumPayloadLimitInBytes) || responseContent.Length > MaximumPayloadLimitInBytes))
				{
					details.Content = new
					{
						Length = headersContentLength,
						Content = LargePayloadMessage
					};
				}
				else
				{
					details.Content = new
					{
						Length = headersContentLength,
						Content = responseContent
					};
				}
			}

			if (response.Headers != null)
			{
				details.RequestId = response.GetRequestId();
			}
			if (response.Headers != null)
			{
				details.Headers = response.Headers;
			}

			details.TotalMilliseconds = stopwatch.Elapsed.TotalMilliseconds;

			return details;
		}
	}
}