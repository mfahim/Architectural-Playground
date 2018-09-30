using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MicroServices.Animal.Api.Data.Domains.ProgramsStatus
{
	[Table("Program", Schema = "Status")]
	public class Program
	{
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Program()
		{
			ProgramStatus = new HashSet<ProgramStatus>();
		}

		public short ProgramID { get; set; }

		[Required]
		[StringLength(10)]
		public string ProgramCode { get; set; }

		[Required]
		[StringLength(255)]
		public string Description { get; set; }

		public bool IsActive { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime CreatedDate { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime LastModifiedDate { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<ProgramStatus> ProgramStatus { get; set; }
	}
}