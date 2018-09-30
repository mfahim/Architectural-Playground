using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.Animal
{
	[Table("Movement", Schema = "Animal")]
	public class AnimalMovement
	{
		[Key]
		public long MovementID { get; set; }

		public long AnimalID { get; set; }

		public int? FromPropertyIdentifierID { get; set; }

		public int? ToPropertyIdentifierID { get; set; }

		public DateTime MovementDate { get; set; }

		[StringLength(25)]
		public string NVDSerialNumber { get; set; }

		public bool SourceNotMatched { get; set; }

		public long CreatedRequestID { get; set; }

		public long? LastModifiedRequestID { get; set; }

		public byte MovementTypeId { get; set; }

		public virtual Animal Animal { get; set; }
	}
}