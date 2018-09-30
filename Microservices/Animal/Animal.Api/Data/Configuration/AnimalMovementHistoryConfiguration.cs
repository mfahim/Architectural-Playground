using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroServices.Animal.Api.Data.Domains.Animal;

namespace MicroServices.Animal.Api.Data.Configuration
{
	public class AnimalMovementHistoryConfiguration : IEntityTypeConfiguration<AnimalMovementHistory>
	{
		public void Configure(EntityTypeBuilder<AnimalMovementHistory> builder)
		{
			builder.Property(e => e.NvdSerialReference).IsUnicode(false);
		}
	}
}