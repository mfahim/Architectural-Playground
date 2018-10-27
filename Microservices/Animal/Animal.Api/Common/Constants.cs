using System.ComponentModel;

namespace MicroServices.Animal.Api.Common
{
	public static class Constants
	{
		public enum AnimalCacheKey
		{
			BusinessRulesList,
			ReachablePropertyListForAccount,
			PropertyIdentifierByPIC,
			PropertyIdentifierByEstablishmentNumber,
			PropertyIdentifierByAssociatedPIC
		}
	}

	public class BusinessRule
	{
		public enum OutcomeStatus
		{
			Silent = 1,

			Info = 2,

			Warning = 3,

			Error = 4
		}

	}

	public class Animal
	{
		public enum ExcludedReason
		{
			Processed = 1,
			NaturalCauses = 2,
			Destroyed = 3,
			LiveExport = 4,
			LostOrStolen = 5,
			ReportedToPoliceAsStolen = 6
		}

		public enum Species
		{
			[Description("Cattle")]
			C = 1,
			[Description("Sheep")]
			S,
			[Description("Goat")]
			G,
			[Description("Alpaca")]
			A,
			[Description("Pig")]
			P,
			[Description("Unknown")]
			U
		}
	}
}