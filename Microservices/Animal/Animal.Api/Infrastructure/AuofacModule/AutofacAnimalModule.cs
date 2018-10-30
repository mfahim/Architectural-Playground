using Autofac;
using Microsoft.EntityFrameworkCore.Design;
using MicroServices.Animal.Api.Data;
using MicroServices.Animal.Api.Data.Factories;
using MicroServices.Animal.Api.Features.Animal.Controller;
using MicroServices.Animal.Api.Infrastructure.Configuration;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;

namespace MicroServices.Animal.Api.Infrastructure.AuofacModule
{
	public class AutofacAnimalModule : Module
	{
		private static IConsulService GetConsul(IComponentContext ctx)
		{
			return ctx.Resolve<IConsulService>();
		}

		private static ICachingService GetCaching(IComponentContext ctx)
		{
			return ctx.Resolve<ICachingService>();
		}

		private static IAnimalMicroServiceSetting GetAnimalSetting(IComponentContext ctx)
		{
			return ctx.Resolve<IAnimalMicroServiceSetting>();
		}

		private static ILogger GetLogger(IComponentContext ctx)
		{
			return ctx.Resolve<ILogger>();
		}

		private static IConfigurationSetting GetConfigSetting(IComponentContext ctx)
		{
			return ctx.Resolve<IConfigurationSetting>();
		}

		private static IDesignTimeDbContextFactory<AnimalContext> GetContextFactory(IComponentContext ctx)
		{
			return ctx.Resolve<IDesignTimeDbContextFactory<AnimalContext>>();
		}

		private string GetRefreshConnectionString(IComponentContext ctx)
		{
			return GetConsul(ctx).Get("ConnectionString").GetAwaiter().GetResult();
		}

		private string GetLegacyConnectionString(IComponentContext ctx)
		{
			return GetConsul(ctx).Get("LegacyConnectionString").GetAwaiter().GetResult();
		}

		private string GetTransactionType(IComponentContext ctx)
		{
			return GetConsul(ctx).Get("Features/Animal/TransactionType").GetAwaiter().GetResult();
		}

		private string GetQueryType(IComponentContext ctx)
		{
			return GetConsul(ctx).Get("Features/Animal/QueryType").GetAwaiter().GetResult();
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<AnimalDbContextFactory>().As<IDesignTimeDbContextFactory<AnimalContext>>()
				.InstancePerLifetimeScope();

			builder.Register(ctx =>
				{
					var connectionString = GetRefreshConnectionString(ctx);
					return new AnimalDapperFactory(connectionString);
				})
				.As<IDapperConnectionFactory>().InstancePerLifetimeScope();
		}
	}
}