using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MicroServices.Animal.Api.Data.Domains.Device
{
	[Table("DeviceDefinition", Schema = "Reference")]
	public class DeviceDefinition
	{
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public DeviceDefinition()
		{
			Device = new HashSet<Device>();
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public byte DefinitionID { get; set; }

		[Required]
		[StringLength(1)]
		public string DeviceType { get; set; }

		[Required]
		[StringLength(255)]
		public string Description { get; set; }

		public byte SpeciesID { get; set; }

		public bool IsBreederDevice { get; set; }

		[NotMapped]
		public bool IsPostBreederDevice => !IsBreederDevice;

		public byte ColourID { get; set; }

		public bool IsYearColoured { get; set; }

		public bool IsActive { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime CreatedDate { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime? LastModifiedDate { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Device> Device { get; set; }
	}
}