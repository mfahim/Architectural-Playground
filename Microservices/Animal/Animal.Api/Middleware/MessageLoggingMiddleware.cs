using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using MicroServices.Animal.Api.Infrastructure.AuofacModule;
using MicroServices.Animal.Api.Infrastructure.Configuration;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;
using Serilog.Events;

namespace MicroServices.Animal.Api.Middleware
{
	/// <summary>
	///     middleware for the logging, this should be put as the first middleware in the chain so that all the
	///     request/response are being captured
	/// </summary>
	public class MessageLoggingMiddleware
	{
		public static string RequestIdKey = "requestId";
		public static string ReferenceEndpointKey = "ReferenceEndpoint";
		private readonly IConfigurationSetting _configurationSetting;
		private readonly ILogger _logger;
		private readonly RequestDelegate _next;

		public MessageLoggingMiddleware(IConfigurationSetting configurationSetting, ILogger logger, RequestDelegate next)
		{
			_configurationSetting = configurationSetting;
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_next = next ?? throw new ArgumentNullException(nameof(next));
		}

		public async Task InvokeAsync(HttpContext context, IConfigurationSetting setting)
		{
			try
			{
				// if the request id doesn't exist, it is not coming from the API, we can ignore that.
				if (!context.Request.Headers.ContainsKey(RequestIdKey) &&
				    context.Request.GetRequestUri().ToString().Contains("heartbeat"))
				{
					await _next(context);
					return;
				}

				var request = context.Request;

				if (!request.Headers.ContainsKey(HttpHeaders.InternalRequestIdHeader))
				{
					request.Headers.Add(HttpHeaders.InternalRequestIdHeader,
						Guid.NewGuid().ToString());
					request.Headers.Add(ReferenceEndpointKey, request.GetUri().ToString());
				}

				var stopwatch = Stopwatch.StartNew();

				// needs to capture the content before passing to the pipeline since the stream will be at the end
				// when the processing finish
				var requestContent = string.Empty;
				if (!string.Equals(request.Method, WebRequestMethods.Http.Get, StringComparison.OrdinalIgnoreCase))
					requestContent = await FormatRequest(request);


				var originalBodyStream = context.Response.Body;
				var needToCaptureResponse = string.Empty;
				var needToCapture = !string.Equals(request.Method, WebRequestMethods.Http.Get, StringComparison.OrdinalIgnoreCase);

				if (needToCapture)
				{
					using (var responseBody = new MemoryStream())
					{
						context.Response.Body = responseBody;
						try
						{
							await _next(context);
							stopwatch.Stop();
							needToCaptureResponse = await FormatResponse(context.Response);
							await responseBody.CopyToAsync(originalBodyStream);
						}
						finally
						{
							// to make sure the caller of this middleware can still manipulate the
							// content in the response stream
							context.Response.Body = originalBodyStream;
						}
					}
				}
				else
				{
					await _next(context);
					stopwatch.Stop();
				}


				await LogDetails(request, context.Response, stopwatch, requestContent, needToCaptureResponse);
			}
			catch (Exception ex)
			{
				await _logger.Error(new ApplicationLogMessage(setting, context.Request.GetRequestId1()?.ToString(),
					context.Request.GetInternalRequestId1(), "Error occured while trying to perform logging.", LogEventLevel.Error,
					ex));
			}
		}

		private async Task LogDetails(HttpRequest request, HttpResponse response, Stopwatch stopwatch, string requestContent,
			string responseContent)
		{
			var message = new HttpRequestResponseLogMessage(_configurationSetting, request, response, stopwatch, requestContent,
				responseContent);
			await _logger.Info(message);
		}

		private async Task<string> FormatRequest(HttpRequest request)
		{
			request.EnableRewind();
			var body = request.Body;

			var buffer = new byte[Convert.ToInt32(request.ContentLength)];
			await request.Body.ReadAsync(buffer, 0, buffer.Length);
			var bodyAsText = Encoding.UTF8.GetString(buffer);
			body.Seek(0, SeekOrigin.Begin);
			request.Body = body;

			return bodyAsText;
		}

		private async Task<string> FormatResponse(HttpResponse response)
		{
			response.Body.Seek(0, SeekOrigin.Begin);
			var text = await new StreamReader(response.Body).ReadToEndAsync();
			response.Body.Seek(0, SeekOrigin.Begin);

			return text;
		}
	}
}