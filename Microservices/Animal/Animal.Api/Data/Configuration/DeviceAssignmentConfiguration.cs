using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroServices.Animal.Api.Data.Domains.Device;

namespace MicroServices.Animal.Api.Data.Configuration
{
	public class DeviceAssignmentConfiguration : IEntityTypeConfiguration<DeviceAssignment>
	{
		public void Configure(EntityTypeBuilder<DeviceAssignment> builder)
		{
			builder.HasOne(e => e.Animal).WithMany(e => e.DeviceAssignment).HasForeignKey(e => e.AnimalID);

			builder.HasOne(e => e.Device).WithMany(e => e.DeviceAssignment).HasForeignKey(e => e.DeviceID);
		}
	}
}