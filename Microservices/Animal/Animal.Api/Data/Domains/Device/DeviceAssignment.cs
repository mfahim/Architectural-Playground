using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MicroServices.Animal.Api.Data.Domains.Device
{
	[Table("DeviceAssignment", Schema = "Animal")]
	public class DeviceAssignment
	{
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public DeviceAssignment()
		{
		}

		public long DeviceID { get; set; }

		public long AnimalID { get; set; }

		public DateTime AssignmentDate { get; set; }

		public DateTime? ReplacementDate { get; set; }

		public long CreatedRequestID { get; set; }

		public long? LastModifiedRequestID { get; set; }

		[Key]
		public long DeviceAssignmentID { get; set; }

		public virtual Animal.Animal Animal { get; set; }

		public virtual Device Device { get; set; }
	}
}