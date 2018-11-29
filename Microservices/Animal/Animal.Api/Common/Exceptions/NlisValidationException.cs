using System.Linq;
using System.Net;
using MicroServices.Animal.Api.Common.Cqrs;
using MicroServices.Animal.Api.Infrastructure.Configuration;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;

namespace MicroServices.Animal.Api.Common.Exceptions
{
	public class NlisValidationException
	{
		public static RestContent CreateRestContent(RuleResultModel[] ruleResults)
		{
			if (ruleResults != null)
			{
				var errors = ruleResults.Where(p => p.ValidationOutcome.OutcomeStatus == BusinessRule.OutcomeStatus.Error.ToString());
				var infos = ruleResults.Where(p => p.ValidationOutcome.OutcomeStatus == BusinessRule.OutcomeStatus.Info.ToString());
				var warnings = ruleResults.Where(p => p.ValidationOutcome.OutcomeStatus == BusinessRule.OutcomeStatus.Warning.ToString());

				return RestContent.CreateWithErrors(
					errors.Any() ? errors.Select(p => p.ValidationOutcome) : null,
					warnings.Any() ? warnings.Select(p => p.ValidationOutcome) : null,
					infos.Any() ? infos.Select(p => p.ValidationOutcome) : null
				);
			}
			return RestContent.CreateWithGenericError("A validation error occurred.");
		}
		public static HttpStatusCode HttpStatus() => HttpStatusCode.OK;
	}
}