using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Rest;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;

namespace MicroServices.Animal.Api.Common.Cqrs.Rules
{
	public class BusinessRulePipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
		where TResponse : BaseBusinessRuleQueryResponse
	{
		private readonly IBusinessRule<TRequest, TResponse>[] _businessRuleRequest;

		public BusinessRulePipelineBehavior(IBusinessRule<TRequest, TResponse>[] businessRuleRequest)
		{
			_businessRuleRequest = businessRuleRequest;
		}

		public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
			RequestHandlerDelegate<TResponse> next)
		{
			var animalResponseQuery = await next();
			var tasks = _businessRuleRequest.Select(x => x.Evaluate(request, animalResponseQuery)).ToArray();
			RuleResultModel[] ruleResults;

			try
			{
				// run applicable business rules
				ruleResults = await Task.Run(() => Task.WhenAll(tasks));
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"An error occured while running business rules.{ex.Message}. {ex.StackTrace}",
					ex);
			}

			var firstError = ruleResults.FirstOrDefault(p =>
				p.IsSatisfied == false && p.ValidationOutcome.OutcomeStatus == BusinessRule.OutcomeStatus.Error.ToString());

			// if validation failed, raise the error & cancel the command
			// todo : cancelation token needs to be activate in here
			if (firstError != null)
				throw new ValidationException(firstError.ValidationOutcome.Message, string.Empty, ruleResults);

			var worthReturingRuleOucomes = new[]
			{
				BusinessRule.OutcomeStatus.Warning.ToString(), BusinessRule.OutcomeStatus.Info.ToString()
			};

			var infoAndWarningRules =
				ruleResults.Where(rl => worthReturingRuleOucomes.Contains(rl.ValidationOutcome.OutcomeStatus)).ToArray();

			// keep the rule results, in case of info/warnings
			animalResponseQuery.RuleResults = infoAndWarningRules;
			return animalResponseQuery;
		}
	}
}