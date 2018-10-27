using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using MicroServices.Animal.Api.Data.Domains.ProgramsStatus;

namespace MicroServices.Animal.Api.Data.Domains.Property
{
	[Table("PropertyAnimalStatusRule", Schema = "Status")]
	public class PropertyAnimalStatusRule
	{
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public PropertyAnimalStatusRule()
		{
		}

		public long PropertyAnimalStatusRuleID { get; set; }

		public int PropertyIdentifierID { get; set; }

		public short ProgramStatusID { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime RuleActivationDate { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime? RuleInactivationDate { get; set; }

		public short? WithHoldDurationPostOFFPropertyMovement { get; set; }

		[StringLength(256)]
		public string Comments { get; set; }

		public long CreatedRequestID { get; set; }

		public long? LastModifiedRequestID { get; set; }

		public virtual ProgramStatus ProgramStatus { get; set; }
	}
}