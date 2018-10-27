using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.Property
{
	[Table("Identifier", Schema = "Property")]
	public class PropertyIdentifier
	{
		[Key]
		public int IdentifierID { get; set; }

		[Required]
		[StringLength(8)]
		public string IdentifierCode { get; set; }

		[ForeignKey("IdentifierTypeID")]
		public virtual PropertyIdentifierType IdentifierType { get; set; }

		public byte IdentifierTypeID { get; set; }

		public int PropertyID { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime EffectiveFrom { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime? EffectiveTo { get; set; }

		public long CreatedRequestID { get; set; }

		public long? LastModifiedRequestID { get; set; }

	}
}