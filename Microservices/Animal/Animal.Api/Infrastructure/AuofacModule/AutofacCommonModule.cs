using System;
using System.Net;
using System.Reflection;
using Autofac;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MicroServices.Animal.Api.Common.Cqrs.Rules;
using MicroServices.Animal.Api.Features.Animal.Controller;
using MicroServices.Animal.Api.Infrastructure.Configuration;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;
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
}