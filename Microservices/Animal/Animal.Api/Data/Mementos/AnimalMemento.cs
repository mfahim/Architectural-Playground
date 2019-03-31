using System;
using MicroServices.Animal.Api.Data.Domains.Shared;

namespace MicroServices.Animal.Api.Data.Mementos
{
	public sealed class AnimalMemento : EntityMemento
	{
		public long AnimalId { get; set; }

		public DateTime? BirthDate { get; set; }
		public byte? SpeciesId { get; }

		public byte? SpeciesID { get; set; }

		public int? CurrentPropertyIdentifierID { get; set; }

		public long CreatedRequestID { get; set; }

		public long? LastModifiedRequestID { get; set; }

		internal AnimalMemento(long animalId, DateTime? birthDate, byte? speciesId, int? currentPropertyIdentifierId, long createRequestId)
		{
			CreatedRequestID = createRequestId;
			AnimalId = animalId;
			BirthDate = birthDate;
			SpeciesId = speciesId;
			CurrentPropertyIdentifierID = currentPropertyIdentifierId;
		}

	}
}