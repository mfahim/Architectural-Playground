using System;
using System.Threading.Tasks;
using MicroServices.Animal.Api.Common.Cqrs;

namespace MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces
{
	public interface ILogMessage
	{
		string ApplicationName { get; }
		string LogEventLevel { get; }
		string RequestId { get; }
		string MachineName { get; }
		object Request { get; }
		object Response { get; }
		string Message { get; }
		string InternalRequestId { get; }
		string UserId { get; }
		string ClientId { get; }
		string ClientIpAddress { get; }
		DateTime Time { get; }
		double? ProcessingTime { get; }
		string RequestUri { get; }
		string Exception { get; }
		string Endpoint { get; }
	}

	public interface IBusinessRule<TRequest, TResponse>
	{
		Task<RuleResultModel> Evaluate(TRequest request, TResponse queryResponse);
		string RuleId { get; }
	}

	// placeholder for future animal settings
	public interface IAnimalMicroServiceSetting : IConfigurationSetting
	{
	}
}