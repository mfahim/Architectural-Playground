using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.Animal
{
	[Table("AnimalAudit", Schema = "Animal")]
	public class AnimalAudit
	{
		[Key]
		public long AuditID { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime AuditTime { get; set; }

		[Required]
		[StringLength(1)]
		public string AuditAction { get; set; }

		public long? AuditRequestID { get; set; }

		public long? AuditIsUndoForRequestID { get; set; }

		public long? AuditIsUndoneByRequestID { get; set; }

		public long AnimalID { get; set; }

		public DateTime? BirthDate { get; set; }

		public DateTime? ExcludedDate { get; set; }

		public DateTime? OriginDate { get; set; }

		public short? ExcludedReasonID { get; set; }

		public int? OriginPropertyIdentifierID { get; set; }

		public byte? SpeciesID { get; set; }

		public int? CurrentPropertyIdentifierID { get; set; }

		public DateTime? LatestTransferDate { get; set; }

		public virtual Animal Animal { get; set; }
	}
}