using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroServices.Animal.Api.Data.Domains.Reference;

namespace MicroServices.Animal.Api.Data.Domains.ProgramsStatus
{
	[Table("ProgramStatusSpecies", Schema = "Status")]
	public class ProgramStatusSpecies
	{
		[Column(Order = 0)]
		[Key]
		[ForeignKey("ProgramStatus")]
		public short ProgramStatusID { get; set; }

		[Column(Order = 1)]
		[Key]
		[ForeignKey("Species")]
		public byte SpeciesID { get; set; }

		public virtual Species Species { get; set; }

		public virtual ProgramStatus ProgramStatus { get; set; }
	}
}