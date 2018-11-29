using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MicroServices.Animal.Api.Extensions;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Commands;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Queries;
using MicroServices.Animal.Api.Infrastructure.Configuration;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;

namespace MicroServices.Animal.Api.Features.Animal.Controller
{
	[Route("api/v1")]
	public class AnimalController : Microsoft.AspNetCore.Mvc.Controller
	{
		private readonly IMediator _mediator;

		public AnimalController(IConsulService consulService, IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet]
		[Route("animal/{animalId:long}")]
		public async Task<IActionResult> GetAnimalByAnimalId(long animalId)
		{
			var query = new GetAnimalByAnimalIdQuery {AnimalId = animalId};

			var createAnimalResponse = await _mediator.Send(query);

			var restResult = createAnimalResponse.ToRestResult(HttpStatusCode.NotFound);

			return ObjectResultExtension.CreateResponse(restResult);
		}

		[HttpGet]
		[Route("animal/{deviceCompositeKey}")]
		public async Task<IActionResult> Get(string deviceCompositeKey)
		{
			var query = new GetAnimalByDeviceIdentifierQuery {RequestId = Request.GetRequestId1()};

			var deviceIdentifier = deviceCompositeKey.GetDeviceIdentifier();
			if (deviceIdentifier.IsNlisId())
				query.NlisId = deviceIdentifier;
			else
				query.RfId = deviceIdentifier;

			var createAnimalResponse = await _mediator.Send(query);
			var restResult = createAnimalResponse.ToRestResult(HttpStatusCode.NotFound);

			return ObjectResultExtension.CreateResponse(restResult);
		}


		[HttpPost]
		[Route("animal")]
		public async Task<IActionResult> Post([FromBody] AnimalPayload payload)
		{
			// todo : enable async-preferred for animal-post queue
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var createAnimalResponse = await _mediator.Send(CreateAnimalCommand.Create(payload, Request));

			var result = createAnimalResponse.ToRestResult(HttpStatusCode.Created);

			return ObjectResultExtension.CreateResponse(result);
		}
	}
}