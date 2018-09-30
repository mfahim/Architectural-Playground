using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MicroServices.Animal.Api.Data.Domains.Device
{
	[Table("DeviceExcludedReason", Schema = "Reference")]
	public class ExcludedReason
	{
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public ExcludedReason()
		{
			Device = new HashSet<Device>();
		}

		[Key]
		public byte ExcludedReasonID { get; set; }

		[Required]
		[StringLength(20)]
		public string ShortCode { get; set; }

		[Required]
		[StringLength(255)]
		public string Description { get; set; }

		public bool IsActive { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime CreatedDate { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime? LastModifiedDate { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Device> Device { get; set; }
	}
}