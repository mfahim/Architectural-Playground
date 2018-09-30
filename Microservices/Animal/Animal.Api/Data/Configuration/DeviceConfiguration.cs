using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroServices.Animal.Api.Data.Domains.Device;

namespace MicroServices.Animal.Api.Data.Configuration
{
	public class DeviceConfiguration : IEntityTypeConfiguration<Device>
	{
		public void Configure(EntityTypeBuilder<Device> builder)
		{
			builder.Property(e => e.RFID).IsUnicode(false);
			builder.Property(e => e.NLISID).IsUnicode(false);
			builder.Property(e => e.ProductVariation).IsUnicode(false);
			builder.Property(e => e.ManagementTagID).IsUnicode(false);
			builder.HasMany(e => e.DeviceAssignment).WithOne(e => e.Device);
		}
	}
}