using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.Animal
{
	[Table("MovementHistory", Schema = "Animal")]
	public class AnimalMovementHistory
	{
		[Key]
		public int MovementHistoryID { get; set; }

		public long? AnimalID { get; set; }

		public int? FromPropertyIdentifierID { get; set; }

		public int? PropertyIdentifierID { get; set; }

		public DateTime? DateOn { get; set; }

		public DateTime? DateOff { get; set; }

		public int? ToPropertyIdentifierID { get; set; }

		[StringLength(25)]
		public string NvdSerialReference { get; set; }

		public long CreatedRequestID { get; set; }

		public long? LastModifiedRequestID { get; set; }

		public byte MovementTypeId { get; set; }

		public bool? IsDeleted { get; set; }

		public virtual Animal Animal { get; set; }
	}
}