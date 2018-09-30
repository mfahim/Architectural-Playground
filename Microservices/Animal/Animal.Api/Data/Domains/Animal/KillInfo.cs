using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.Animal
{
	[Table("KillInfo", Schema = "Animal")]
	public class KillInfo
	{
		public long KillInfoID { get; set; }
		public DateTime KillDate { get; set; }
		public string ProcessorEstablishmentNumber { get; set; }
		public byte ChainNumber { get; set; }
		public int BodyNumber { get; set; }
		public int? ConsignmentPropertyIdentifierID { get; set; }
		public string NVDNumber { get; set; }
		public string OperatorNumber { get; set; }
		public string LotNumber { get; set; }
		public TimeSpan? KillTime { get; set; }
		public byte? SpeciesID { get; set; }
		public string AnimalIdentifier { get; set; }
		public long CreatedRequestID { get; set; }
		public long? LastModifiedRequestID { get; set; }
	}
}