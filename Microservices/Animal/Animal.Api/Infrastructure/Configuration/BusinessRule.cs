using System.Xml.Serialization;
using MicroServices.Animal.Api.Common.Cqrs;
using Newtonsoft.Json;

namespace MicroServices.Animal.Api.Infrastructure.Configuration
{
	public class BaseResponseType
	{
		[JsonIgnore]
		[XmlIgnore]
		public RuleResultModel[] RuleResults { get; }

		public BaseResponseType(RuleResultModel[] ruleResults = null)
		{
			RuleResults = ruleResults;
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
}