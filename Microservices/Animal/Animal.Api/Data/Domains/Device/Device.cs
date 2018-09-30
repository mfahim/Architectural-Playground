using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MicroServices.Animal.Api.Data.Domains.Device
{
	[Table("Device", Schema = "Device")]
	public class Device
	{
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Device()
		{
			DeviceAssignment = new HashSet<DeviceAssignment>();
		}

		public long DeviceID { get; set; }

		[Required]
		[StringLength(16)]
		public string RFID { get; set; }

		[StringLength(16)]
		public string NLISID { get; set; }

		public bool IsNLISDevice { get; set; }

		public byte? DefinitionID { get; set; }

		public short? ICARProductID { get; set; }

		[StringLength(10)]
		public string ProductVariation { get; set; }

		[StringLength(100)]
		public string ManagementTagID { get; set; }

		public int AssignedToPropertyIdentifierID { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime? AssignmentDate { get; set; }

		public byte? ExcludedReasonID { get; set; }

		[Column(TypeName = "date")]
		public DateTime? ExcludedDate { get; set; }

		public long CreatedRequestID { get; set; }

		public long? LastModifiedRequestID { get; set; }

		public int IssueToPropertyIdentifierID { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime IssueDate { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<DeviceAssignment> DeviceAssignment { get; set; }

		public virtual DeviceDefinition DeviceDefinition { get; set; }

		public virtual ExcludedReason DeviceExcludedReason { get; set; }
	}
}