using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using MicroServices.Animal.Api.Data.Domains.Device;
using MicroServices.Animal.Api.Data.Mementos;

namespace MicroServices.Animal.Api.Data.Domains.Animal
{
	[Table("Animal", Schema = "Animal")]
	public class Animal
	{
		public Animal(long animalId, DateTime? birthDate, byte? speciesId, int? currentPropertyIdentifierId)
		{
			AnimalId = animalId;
			BirthDate = birthDate;
			SpeciesId = speciesId;
			CurrentPropertyIdentifierId = currentPropertyIdentifierId;
			DeviceAssignment = new HashSet<DeviceAssignment>();
			Movement = new HashSet<AnimalMovement>();
			MovementHistory = new HashSet<AnimalMovementHistory>();
		}

		public long AnimalId { get; set; }

		public DateTime? BirthDate { get; set; }
		public byte? SpeciesId { get; }
		public int? CurrentPropertyIdentifierId { get; }

		public byte? SpeciesID { get; set; }

		public int? CurrentPropertyIdentifierID { get; set; }

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

		public static Animal FromMemento(AnimalMemento memento)
		{
			var entity = new Animal(
				memento.AnimalId,
				memento.BirthDate,
				memento.SpeciesId,
				memento.CurrentPropertyIdentifierID
			);
			return entity;
		}
	}
}