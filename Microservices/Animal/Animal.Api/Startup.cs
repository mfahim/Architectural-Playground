using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MicroServices.Animal.Api.Infrastructure.AuofacModule;
using MicroServices.Animal.Api.Infrastructure.Configuration;
using MicroServices.Animal.Api.Middleware;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace MicroServices.Animal.Api
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", true, true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
				.AddEnvironmentVariables();
			HostingEnvironment = env;
			Configuration = builder.Build();
		}

		public IConfiguration Configuration { get; }
		private IHostingEnvironment HostingEnvironment { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// load and add MicroServiceSetting section from appsettings.config
			services.Configure<MicroServiceSetting>(Configuration.GetSection("MicroServiceSetting"));

			// todo: enable Polly policy
			//var registry = services.AddPolicyRegistry();

			//var timeout = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));
			//var longTimeout = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30));

			//registry.Add("regular", timeout);
			//registry.Add("long", longTimeout);

			services.AddMvc()
				.AddJsonOptions(p =>
				{
					p.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
					p.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
					p.SerializerSettings.ContractResolver = new DefaultContractResolver();
					p.SerializerSettings.Converters.Add(new StringEnumConverter {CamelCaseText = true});
				});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();

			// the sequence of the middleware is important!
			app.UseMiddleware<HealthCheckMiddleware>("/healthcheck");
			app.UseMiddleware<MessageLoggingMiddleware>();
			app.UseMiddleware<ExceptionHandlingMiddleware>();
			app.UseMvc();
			//app.UseSwagger();
			//app.UseSwaggerUI(c =>
			//{
			//	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Raodee API V1");
			//	c.ShowRequestHeaders();
			//});
		}

		public void ConfigureContainer(ContainerBuilder builder)
		{
			builder.RegisterModule(new AutofacCommonModule(HostingEnvironment));
			builder.RegisterModule(new AutofacAnimalModule());
		}
	}
}