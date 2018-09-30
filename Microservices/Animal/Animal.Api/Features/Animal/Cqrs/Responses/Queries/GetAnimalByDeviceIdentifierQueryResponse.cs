using MicroServices.Animal.Api.Common.Cqrs;
using MicroServices.Animal.Api.Data.Projections;
using MicroServices.Animal.Api.Infrastructure.Configuration;

namespace MicroServices.Animal.Api.Features.Animal.Cqrs.Responses.Queries
{
	public class GetAnimalByDeviceIdentifierQueryResponse : BaseResponseType
	{
		public GetAnimalByDeviceIdentifierQueryResponse(DeviceAssignedToAnimal deviceAssignedToAnimal,
			RuleResultModel[] ruleResults = null) :
			base(ruleResults)
		{
			DeviceAssignedToAnimal = deviceAssignedToAnimal;
		}

		public DeviceAssignedToAnimal DeviceAssignedToAnimal { get; set; }
	}
}