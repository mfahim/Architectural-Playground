using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.Animal
{
	//  Just selected 4 fields of this table as we don't need other fileds
	[Table("vwEUVDSerialNumber", Schema = "Animal")]
	public class EUVDSerialNumber
	{
		[Key]
		public int LPAUpldId { get; set; }

		public string StartCharacter { get; set; }

		public long StartNumber { get; set; }

		public long EndNumber { get; set; }
	}
}