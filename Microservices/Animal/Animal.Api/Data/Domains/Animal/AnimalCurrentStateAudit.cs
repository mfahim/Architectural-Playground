using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.Animal
{
	[Table("AnimalCurrentStateAudit", Schema = "Animal")]
	public class AnimalCurrentStateAudit
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

		public bool LTStatus { get; set; }

		public bool EUStatus { get; set; }
	}
}