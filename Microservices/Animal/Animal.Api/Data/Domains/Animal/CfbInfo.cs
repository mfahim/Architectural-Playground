using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.Animal
{
	[Table("CfbInfo", Schema = "Animal")]
	public class CfbInfo
	{
		public long CfbInfoID { get; set; }
		public long KillInfoID { get; set; }
		public Nlis.Standard.CommonPackages.Apis.Enums.Animal.CarcaseDocumentType TypeId { get; set; }
		public long CreatedRequestID { get; set; }
		public long? LastModifiedRequestID { get; set; }
	}
}