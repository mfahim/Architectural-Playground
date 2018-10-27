using MicroServices.Animal.Api.Common.Cqrs;
using MicroServices.Animal.Api.Infrastructure.Configuration;

namespace MicroServices.Animal.Api.Features.Animal.Cqrs.Responses.Commands
{
	public class UpdateAnimalCommandResponse : BaseResponseType
	{
		public UpdateAnimalCommandResponse(Data.Domains.Animal.Animal animal, RuleResultModel[] ruleResults = null) :
			base(ruleResults)
		{
			Animal = animal;
		}

		public Data.Domains.Animal.Animal Animal { get; set; }
	}
}