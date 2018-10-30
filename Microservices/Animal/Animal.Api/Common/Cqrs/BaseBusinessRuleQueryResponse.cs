using System.Collections.Specialized;
using MicroServices.Animal.Api.Infrastructure.Configuration;

namespace MicroServices.Animal.Api.Common.Cqrs
{
	public abstract class BaseBusinessRuleQueryResponse
	{
		protected BaseBusinessRuleQueryResponse()
		{
			AllBusinessRules = new NameValueCollection();
		}

		public NameValueCollection AllBusinessRules { get; set; }
		public RuleResultModel[] RuleResults { get; set; }
	}

	public class RuleResultModel
	{
		public bool IsSatisfied { get; private set; }
		public ValidationOutcome ValidationOutcome { get; private set; }

		public RuleResultModel(bool isSatisfied, ValidationOutcome validationOutcome)
		{
			IsSatisfied = isSatisfied;
			ValidationOutcome = validationOutcome;
		}

		public RuleResultModel(bool isSatisfied, string ruleCode, string message = null, BusinessRule.OutcomeStatus outcomeStatus = BusinessRule.OutcomeStatus.Silent)
		{
			IsSatisfied = isSatisfied;

			ValidationOutcome = new ValidationOutcome() { Code = ruleCode, Message = message, OutcomeStatus = outcomeStatus.ToString() };
		}
	}
}