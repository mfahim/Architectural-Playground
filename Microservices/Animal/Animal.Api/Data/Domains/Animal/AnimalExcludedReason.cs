using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MicroServices.Animal.Api.Data.Domains.Animal
{
	[Table("ExcludedReason", Schema = "Reference")]
	public class AnimalExcludedReason
	{
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public AnimalExcludedReason()
		{
		}

		[Key]
		public short ExcludedReasonID { get; set; }

		[Required]
		[StringLength(255)]
		public string Description { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime CreatedDate { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime? LastModifiedDate { get; set; }
	}
}