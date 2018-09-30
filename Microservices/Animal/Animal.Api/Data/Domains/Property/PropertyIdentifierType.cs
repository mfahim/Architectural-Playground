using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.Property
{
	[Table("PropertyIdentifierType", Schema = "Reference")]
	public class PropertyIdentifierType
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public byte IdentifierTypeID { get; set; }

		[Required]
		[StringLength(50)]
		public string ShortCode { get; set; }

		[StringLength(255)]
		public string Description { get; set; }

		public bool IsActive { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime CreatedDate { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime? LastModifiedDate { get; set; }
	}
}