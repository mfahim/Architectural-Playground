using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MicroServices.Animal.Api.Common.Cqrs.Rules;
using MicroServices.Animal.Api.Features.Animal.Controller;
using MicroServices.Animal.Api.Infrastructure.Configuration;
using Newtonsoft.Json;
using ICachingService = MicroServices.Animal.Api.Infrastructure.Configuration.ICachingService;
using IConfigurationSetting = MicroServices.Animal.Api.Infrastructure.Configuration.IConfigurationSetting;
using Module = Autofac.Module;

namespace MicroServices.Animal.Api.Infrastructure.AuofacModule
{
	public class AutofacCommonModule : Module
	{
		private readonly string configRoot;
		private readonly LoggerSinks sinks;

		public AutofacCommonModule(IHostingEnvironment environment)
		{
			if (environment.IsDevelopment())
				sinks = LoggerSinks.ColoredConsole | LoggerSinks.RollingFile | LoggerSinks.RabbitMq;
			else
				sinks = LoggerSinks.RollingFile | LoggerSinks.RabbitMq;
			configRoot = "API Refresh";
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
			builder.RegisterType<StandardHttpClient>().As<IHttpClient>().SingleInstance();

			// register both interfaces with one config class
			builder.Register(ctx => ctx.Resolve<IOptions<MicroServiceSetting>>().Value).As<IAnimalMicroServiceSetting>()
				.SingleInstance();
			builder.Register(ctx => ctx.Resolve<IOptions<MicroServiceSetting>>().Value).As<IConfigurationSetting>()
				.SingleInstance();

			builder.RegisterType<RestResultFactory>().As<IRestResultFactory>().SingleInstance();
			builder.RegisterType<MemoryCachingService>().As<ICachingService>()
				.WithParameter("defaultExpirationTimeSpan", TimeSpan.FromMinutes(60))
				.SingleInstance();

			builder.Register<IConsulService>(ctx =>
			{
				var animalSetting = ctx.Resolve<IAnimalMicroServiceSetting>();
				var httpClient = ctx.Resolve<IHttpClient>();
				return new ConsulService(httpClient, configRoot, TimeSpan.FromMinutes(60),
					IPAddress.Parse(animalSetting.ConsulIpAddress));
			}).SingleInstance();

			builder.Register(x => sinks).As<LoggerSinks>().SingleInstance();

			// register Mediatr
			builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();
			builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
			builder.RegisterGeneric(typeof(BusinessRulePipelineBehavior<,>)).As(typeof(IPipelineBehavior<,>));

			builder.Register<ServiceFactory>(ctx =>
			{
				var c = ctx.Resolve<IComponentContext>();
				object o;
				return t => c.TryResolve(t, out o) ? o : null;
			});

			builder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();

			var mediatrOpenTypes = new[]
			{
				typeof(IRequestHandler<,>),
				typeof(IRequestHandler<,>),
				typeof(IRequestPreProcessor<>), // register pre-command actions
				typeof(AsyncRequestHandler<>),
				typeof(INotificationHandler<>),
				typeof(IBusinessRule<,>)
			};

			foreach (var mediatrOpenType in mediatrOpenTypes)
				builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
					.AsClosedTypesOf(mediatrOpenType)
					.AsImplementedInterfaces();
		}
	}
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

	[Flags]
	public enum LoggerSinks
	{
		None = 0x0,
		RabbitMq = 0x1,
		RollingFile = 0x2,
		ColoredConsole = 0x4
	}

	public class StandardHttpClient : IHttpClient
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private HttpClient _client;

		public StandardHttpClient(IHttpContextAccessor httpContextAccessor)
		{
			_client = new HttpClient();
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
		{
			var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

			var response = await SendAsync(requestMessage);

			return await response.Content.ReadAsStringAsync();
		}

		private async Task<HttpResponseMessage> DoPostPutAsync<T>(HttpMethod method, string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
		{
			if (method != HttpMethod.Post && method != HttpMethod.Put)
			{
				throw new ArgumentException("Value must be either post or put.", nameof(method));
			}

			// a new StringContent must be created for each retry
			// as it is disposed after each call

			var requestMessage = new HttpRequestMessage(method, uri);

			requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");

			await SendAsync(requestMessage, authorizationToken, requestId);

			return await _client.SendAsync(requestMessage);

		}


		public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
		{
			return await DoPostPutAsync(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod);
		}

		public async Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
		{
			return await DoPostPutAsync(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationMethod);
		}

		public async Task<HttpResponseMessage> PatchAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
		{
			return await DoPostPutAsync(new HttpMethod("PATCH"), uri, item, authorizationToken, requestId, authorizationMethod);
		}

		public async Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
		{
			var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

			return await SendAsync(requestMessage, authorizationToken, requestId);
		}


		private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
		{
			var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
			if (!string.IsNullOrEmpty(authorizationHeader))
			{
				requestMessage.Headers.Add("Authorization", new List<string>() { authorizationHeader });
			}
		}

		public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
		{
			SetAuthorizationHeader(requestMessage);

			if (authorizationToken != null)
			{
				requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
			}

			if (requestId != null)
			{
				requestMessage.Headers.Add("x-requestid", requestId);
			}

			try
			{
				var response = await _client.SendAsync(requestMessage);

				return response;

			}
			catch (Exception ex)
			{
				throw new HttpRequestException($"{requestMessage.Method} request to '{requestMessage.RequestUri}' failed", ex);
			}
		}

	}

}