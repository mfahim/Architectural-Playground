using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroServices.Animal.Api.Data.Domains.Animal;

namespace MicroServices.Animal.Api.Data.Configuration
{
	public class AnimalAuditConfiguration : IEntityTypeConfiguration<AnimalAudit>
	{
		public void Configure(EntityTypeBuilder<AnimalAudit> builder)
		{
			builder.Property(e => e.AuditAction).IsUnicode(false);
		}
	}
}