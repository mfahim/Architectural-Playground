using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.Animal
{
	[Table("AnimalCurrentState", Schema = "Animal")]
	public class AnimalCurrentState
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long AnimalID { get; set; }

		public bool LTStatus { get; set; }

		public bool EUStatus { get; set; }

		public virtual Animal Animal { get; set; }
	}
}