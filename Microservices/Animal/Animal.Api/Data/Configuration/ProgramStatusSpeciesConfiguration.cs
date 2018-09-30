using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroServices.Animal.Api.Data.Domains.ProgramsStatus;

namespace MicroServices.Animal.Api.Data.Configuration
{
	public class ProgramStatusSpeciesConfiguration : IEntityTypeConfiguration<ProgramStatusSpecies>
	{
		public void Configure(EntityTypeBuilder<ProgramStatusSpecies> builder)
		{
			builder.HasKey(e => new {e.SpeciesID, e.ProgramStatusID});
		}
	}
}