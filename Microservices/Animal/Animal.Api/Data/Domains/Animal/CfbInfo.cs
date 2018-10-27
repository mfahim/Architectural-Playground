using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.Animal
{
	[Table("CfbInfo", Schema = "Animal")]
	public class CfbInfo
	{
		public long CfbInfoID { get; set; }
		public long KillInfoID { get; set; }
		public long CreatedRequestID { get; set; }
		public long? LastModifiedRequestID { get; set; }
	}
}