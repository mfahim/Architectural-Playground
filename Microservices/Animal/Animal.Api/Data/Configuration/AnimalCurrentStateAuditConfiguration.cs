using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroServices.Animal.Api.Data.Domains.Animal;

namespace MicroServices.Animal.Api.Data.Configuration
{
	public class AnimalCurrentStateAuditConfiguration : IEntityTypeConfiguration<AnimalCurrentStateAudit>
	{
		public void Configure(EntityTypeBuilder<AnimalCurrentStateAudit> builder)
		{
			builder.Property(e => e.AuditAction).IsUnicode(false);
		}
	}
}