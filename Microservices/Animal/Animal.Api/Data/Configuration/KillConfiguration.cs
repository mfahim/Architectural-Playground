using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroServices.Animal.Api.Data.Domains.Animal;

namespace MicroServices.Animal.Api.Data.Configuration
{
	public class KillConfiguration : IEntityTypeConfiguration<KillInfo>
	{
		public void Configure(EntityTypeBuilder<KillInfo> builder)
		{
			builder.HasKey(e => e.KillInfoID);
			builder.Property(e => e.CreatedRequestID);
			builder.Property(e => e.LastModifiedRequestID);
		}
	}
}