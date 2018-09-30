using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroServices.Animal.Api.Data.Domains.Device;

namespace MicroServices.Animal.Api.Data.Configuration
{
	public class DeviceDefinitionConfiguration : IEntityTypeConfiguration<DeviceDefinition>
	{
		public void Configure(EntityTypeBuilder<DeviceDefinition> builder)
		{
			builder.Property(e => e.DeviceType);
			builder.Property(e => e.Description).IsUnicode(false);
		}
	}
}