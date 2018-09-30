using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.Reference
{
	[Table("Species", Schema = "Reference")]
	public class Species
	{
		public byte SpeciesID { get; set; }
		public string ShortCode { get; set; }
		public bool IsActive { get; set; }
	}
}