using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroServices.Animal.Api.Data.Domains.ProgramsStatus
{
	[Table("ProgramStatusAction", Schema = "Status")]
	public class ProgramStatusAction
	{
		public int ProgramStatusActionID { get; set; }
		public short ProgramStatusID { get; set; }
		public byte ProgramStatusActionTypeID { get; set; }
		public short ProgramStatusActionTriggerID { get; set; }
		public string Description { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime LastModifiedDate { get; set; }
	}
}