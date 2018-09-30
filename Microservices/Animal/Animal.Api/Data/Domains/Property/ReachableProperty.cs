using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.Property
{
	[Table("ReachableProperty", Schema = "Authority")]
	public class ReachableProperty
	{
		public long ReachablePropertyID { get; set; }
		public int AccountID { get; set; }
		public int PropertyIdentifierID { get; set; }
		public DateTime EffectiveFrom { get; set; }
		public DateTime? EffectiveTo { get; set; }
		public bool IsPrimaryAuthority { get; set; }
		public long CreatedRequestID { get; set; }
		public long? LastModifiedRequestID { get; set; }
		public virtual PropertyIdentifier PropertyIdentifier { get; set; }
	}
}