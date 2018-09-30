using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using MicroServices.Animal.Api.Data.Domains.Property;
using Nlis.Standard.CommonPackages.Apis.Enums;

namespace MicroServices.Animal.Api.Data.Domains.ProgramsStatus
{
	[Table("ProgramStatus", Schema = "Status")]
	public class ProgramStatus
	{
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public ProgramStatus()
		{
			PropertyAnimalStatusRule = new HashSet<PropertyAnimalStatusRule>();
		}

		[Key]
		public short ProgramStatusID { get; set; }

		public short ProgramID { get; set; }

		[Required]
		[StringLength(3)]
		public string StatusCode { get; set; }

		[StringLength(255)]
		public string Description { get; set; }

		public byte ProgramStatusTargetID { get; set; }

		public bool IsActive { get; set; }

		public byte ProgramStatusTypeID { get; set; }

		public bool? IsDisease { get; set; }

		public byte? LevelofConfirmationID { get; set; }

		public byte? FoodSafetyID { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime CreatedDate { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime LastModifiedDate { get; set; }

		public bool IsPropertyAnimalStatusAllowed { get; set; }

		public bool IsPropertyAnimalStatusWithholdDurationAllowed { get; set; }

		public bool IsDurationAllowed { get; set; }

		public virtual Program Program { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<PropertyAnimalStatusRule> PropertyAnimalStatusRule { get; set; }

		public virtual ICollection<ProgramStatusSpecies> AllowedSpecies { get; set; }

		public bool Is(ProgramCode programCode,
			Nlis.Standard.CommonPackages.Apis.Enums.ProgramStatus.ProgramStatusCode programStatusCode)
		{
			if (Program == null)
				return false;

			if (StatusCode.ToUpper() == programStatusCode.ToString() && Program.ProgramCode.ToUpper() == programCode.ToString())
				return true;

			return false;
		}
	}
}