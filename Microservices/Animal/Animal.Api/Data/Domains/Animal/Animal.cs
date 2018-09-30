using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MicroServices.Animal.Api.Data.Domains.Device;

namespace MicroServices.Animal.Api.Data.Domains.Animal
{
	[Table("Animal", Schema = "Animal")]
	public class Animal
	{
		public Animal()
		{
			DeviceAssignment = new HashSet<DeviceAssignment>();
			Movement = new HashSet<AnimalMovement>();
			MovementHistory = new HashSet<AnimalMovementHistory>();
		}

		public long AnimalId { get; set; }

		public DateTime? BirthDate { get; set; }

		public DateTime? ExcludedDate { get; set; }

		public DateTime? OriginDate { get; set; }

		public short? ExcludedReasonID { get; set; }

		public int OriginPropertyIdentifierID { get; set; }

		public byte? SpeciesID { get; set; }

		public int? CurrentPropertyIdentifierID { get; set; }

		public DateTime? LatestTransferDate { get; set; }

		public long CreatedRequestID { get; set; }

		public long? LastModifiedRequestID { get; set; }

		public virtual AnimalCurrentState AnimalCurrentState { get; set; }

		public virtual ICollection<DeviceAssignment> DeviceAssignment { get; set; }

		public virtual ICollection<AnimalMovement> Movement { get; set; }

		public virtual ICollection<AnimalMovementHistory> MovementHistory { get; set; }

		public DeviceAssignment ActiveDeviceAssignment
		{
			get { return DeviceAssignment.FirstOrDefault(da => da.ReplacementDate == null); }
		}

		public bool IsDeceased => ExcludedReasonID ==
		                          (int) Nlis.Standard.CommonPackages.Apis.Enums.Animal.ExcludedReason.NaturalCauses
		                          || ExcludedReasonID == (int) Nlis.Standard.CommonPackages.Apis.Enums.Animal.ExcludedReason
			                          .Processed;
	}
}