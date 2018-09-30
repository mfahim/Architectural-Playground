using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroServices.Animal.Api.Data.Configuration
{
	public class AnimalConfiguration : IEntityTypeConfiguration<Domains.Animal.Animal>
	{
		public void Configure(EntityTypeBuilder<Domains.Animal.Animal> builder)
		{
			builder.HasMany(e => e.DeviceAssignment).WithOne(e => e.Animal);
			builder.HasMany(e => e.Movement).WithOne(e => e.Animal);
		}
	}
}