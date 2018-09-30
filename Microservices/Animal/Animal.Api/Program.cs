using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace MicroServices.Animal.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("hosting.json", false, true)
				.Build();

			var host = new WebHostBuilder()
				.UseStartup<Startup>()
				.UseKestrel()
				//.UseHttpSys()
				.UseConfiguration(config)
				.UseContentRoot(Directory.GetCurrentDirectory())
				.ConfigureServices(services => services.AddAutofac())
				.Build();

			host.Run();
		}
	}
}