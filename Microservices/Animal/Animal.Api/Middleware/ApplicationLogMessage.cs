using System;
using MicroServices.Animal.Api.Infrastructure.Configuration;
using Serilog.Events;

namespace MicroServices.Animal.Api.Middleware
{
	public class ApplicationLogMessage : ILogMessage
	{
		private readonly IConfigurationSetting _configurationSetting;

		public ApplicationLogMessage(IConfigurationSetting configurationSetting,
			string requestId, string internalRequestId, string message, LogEventLevel logEventLevel)
			: this(configurationSetting, requestId, internalRequestId, message, logEventLevel, (Exception) null)
		{
		}


		public ApplicationLogMessage(IConfigurationSetting configurationSetting,
			string requestId, string internalRequestId, string message, LogEventLevel logEventLevel, Exception exception)
		{
			_configurationSetting = configurationSetting;
			RequestId = requestId;
			InternalRequestId = internalRequestId;
			Message = message;
			LogEventLevel = logEventLevel.ToString();
			Exception = exception?.ToString();
		}

		public string ApplicationName => _configurationSetting.ApplicationName;
		public string LogEventLevel { get; }
		public string RequestId { get; }
		public string MachineName => Environment.MachineName;
		public object Request => null;
		public object Response => null;
		public string Message { get; }
		public string InternalRequestId { get; }
		public string UserId => null;
		public string ClientId => null;
		public string ClientIpAddress => null;
		public DateTime Time => DateTime.UtcNow;
		public double? ProcessingTime => null;
		public string RequestUri => null;
		public string Exception { get; }

		public string Endpoint => null;
	}
}