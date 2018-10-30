using System.Linq;
using System.Net;
using LanguageExt;
using MicroServices.Animal.Api.Common.Cqrs;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Commands;

namespace MicroServices.Animal.Api.Infrastructure.Configuration
{
	public static class NlisEitherExtensions
	{
		public static DispatcherResponse<T> ToResult<T>(this Either<ExceptionResponse, T> @this) where T : BaseResponseType
			=> @this.Match(
				Right: data => new DispatcherResponse<T>(data),
				Left: error => new DispatcherResponse<T>(error));

		public static RestResult ToRestResult<T>(this Either<ExceptionResponse, T> @this,
			HttpStatusCode statusCode = HttpStatusCode.OK) where T : BaseResponseType
		{
			var responseResult = ToResult(@this);
			return responseResult.Succeeded ? CreateRestResultWhenSuccessful(responseResult, responseResult.Data, statusCode)
				: CreateRestResultWhenFailed(responseResult.Exception);
		}

		private static RestResult CreateRestResultWhenFailed(ExceptionResponse exception)
		{
			return new RestResult(RestContent.CreateWithGenericError(exception.ErrorMessage), exception.HttpStatusCode);
		}

		private static RestResult CreateRestResultWhenSuccessful<T>(DispatcherResponse<T> data, object value, HttpStatusCode statusCode) where T : BaseResponseType
		{
			var warnings = Enumerable.Empty<ValidationOutcome>();
			var infos = Enumerable.Empty<ValidationOutcome>();
			var errors = Enumerable.Empty<ValidationOutcome>();
			var ruleResults = data.Data.RuleResults;

			if (ruleResults != null)
			{
				warnings = ruleResults
					.Where(rl => rl.ValidationOutcome.OutcomeStatus.Equals(BusinessRule.OutcomeStatus.Warning.ToString()))
					.Select(p => p.ValidationOutcome).ToList();
				infos = ruleResults
					.Where(rl => rl.ValidationOutcome.OutcomeStatus.Equals(BusinessRule.OutcomeStatus.Info.ToString()))
					.Select(p => p.ValidationOutcome).ToList();

				errors = ruleResults
					.Where(rl => rl.ValidationOutcome.OutcomeStatus.Equals(BusinessRule.OutcomeStatus.Error.ToString()))
					.Select(p => p.ValidationOutcome).ToList();
			}

			var restContent = new RestContent(value,
				errors.Any() ? errors : null, warnings.Any() ? warnings : null, infos.Any() ? infos : null);

			return new RestResult(restContent, statusCode);
		}
	}
}