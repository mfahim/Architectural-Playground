using System;
using System.Net.Http;
using System.Threading.Tasks;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;

namespace MicroServices.Animal.Api.Infrastructure.AuofacModule
{
	public interface ILogger : IDisposable
	{
		Task Debug(HttpRequestMessage request, string message);
		Task Info(HttpRequestMessage request, string message);
		Task Warn(HttpRequestMessage request, string message, Exception exception = null);
		Task Error(HttpRequestMessage request, string message, Exception exception = null);
		Task Fatal(HttpRequestMessage request, string message, Exception exception = null);

		Task Debug(ILogMessage message);
		Task Info(ILogMessage message);
		Task Warn(ILogMessage message, Exception exception = null);
		Task Error(ILogMessage message, Exception exception = null);
		Task Fatal(ILogMessage message, Exception exception = null);
	}
}