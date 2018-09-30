using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroServices.Animal.Api.Data.Domains.Device;
using Device = Nlis.Standard.CommonPackages.Apis.Enums.Device;

namespace MicroServices.Animal.Api.Data.Projections
{
	public class DeviceAssignedToAnimal
	{
		private byte? _species;
		public long DeviceID { get; set; }
		public long AnimalId { get; set; }

		public DateTime? BirthDate { get; set; }

		public DateTime? OriginDate { get; set; }

		[Required]
		[StringLength(16)]
		public string RFID { get; set; }

		[StringLength(16)]
		public string NLISID { get; set; }

		public bool IsNLISDevice => string.IsNullOrEmpty(NLISID) ? false : true;

		[Column(TypeName = "datetime2")]
		public DateTime? AssignmentDate { get; set; }

		public short? ExcludedReasonID { get; set; }

		[Column(TypeName = "date")]
		public DateTime? ExcludedDate { get; set; }

		public int IssueToPropertyIdentifierID { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime IssueDate { get; set; }

		public virtual Domains.Animal.Animal Animal { get; set; }

		public bool AnimalAssignedToDevice => AnimalId != 0;

		public bool IsPostBreederDevice { get; set; }

		public int OriginPropertyIdentifierID { get; set; }
		public int AssignedToPropertyIdentifierID { get; set; }

		public byte? Species
		{
			get
			{
				if (!_species.HasValue)
					_species = (byte) Nlis.Standard.CommonPackages.Apis.Enums.Animal.Species.U;

				return _species.Value;
			}

			set => _species = value;
		}

		public int? CurrentPropertyIdentifierID { get; set; }

		public long CreatedRequestID { get; set; }

		public long? LastModifiedRequestID { get; set; }

		public DeviceAssignment DeviceAssignment { get; set; }

		public bool IsActive
		{
			get
			{
				if (ExcludedReasonID == (short) Device.ExcludedReason.Inactive)
					return false;
				return true;
			}
		}
	}
}