using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.Reference
{
	[Table("BusinessRule", Schema = "Reference")]
	public class BusinessRule
	{
		public int BusinessRuleID { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
	}
}