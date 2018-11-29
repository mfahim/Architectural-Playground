using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MicroServices.Animal.Api.Features;
using MicroServices.Animal.Api.Features.Animal.Controller;

namespace MicroServices.Animal.Api.Data.Factories
{
	public class AnimalDbContextFactory : IDesignTimeDbContextFactory<AnimalContext>
	{
		private readonly string _connectionString;

		public AnimalDbContextFactory(IConsulService consuleService)
		{
			_connectionString = consuleService.Get("ConnectionString").GetAwaiter().GetResult();
		}

		public AnimalContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<AnimalContext>();

			builder.UseSqlServer(_connectionString);

			return new AnimalContext(builder.Options);
		}
	}
}