using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MicroServices.Animal.Api.Extensions;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Commands;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Queries;
using MicroServices.Animal.Api.Infrastructure.Configuration;

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

			return NlisObjectResultExtension.CreateResponse(restResult);
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

			return NlisObjectResultExtension.CreateResponse(restResult);
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

			return NlisObjectResultExtension.CreateResponse(result);
		}
	}

	public interface IConsulService
	{
		Task<string> Get(string key);
		Task<int> GetInt(string key);
		Task<bool> GetBool(string key);

		Task RegisterService(ServiceRegistrationRequest request);
		Task DeregisterService(string serviceId);
	}
	public class ServiceRegistrationRequest
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public IEnumerable<string> Tags { get; set; }
		public string Address { get; set; }
		public int Port { get; set; }
		public bool EnableTagOverride { get; set; }
		public Check Check { get; set; }
	}

	public class Check
	{
		public string DeregisterCriticalServiceAfter { get; set; }
		public string Script { get; set; }
		public string HTTP { get; set; }
		public string Interval { get; set; }
		public string TTL { get; set; }
	}

	public static class StringExtentions
	{
		public static string ToBase64(this string str)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
		}

		public static string FromBase64(this string str)
		{
			if (str == null)
				return null;

			var base64EncodedBytes = Convert.FromBase64String(str);

			return Encoding.UTF8.GetString(base64EncodedBytes);
		}

		public static string ToTitleCase(this string str)
		{
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			return textInfo.ToTitleCase(str);
		}

		public static Dictionary<String, String> ExtractParameterValues(this string str)
		{
			var result = new Dictionary<String, String>();
			var parameters = str
				.Replace("(", "")
				.Replace(")", "")
				.Split(',');
			foreach (var param in parameters)
			{
				var paramArray = param.Split('=');
				result.Add(paramArray[0].Trim(), paramArray.Length >= 2 ? paramArray[1].Trim() : string.Empty);
			}
			return result;
		}

		public static string GetDeviceIdentifier(this string deviceCompositeKey)
		{
			var deviceKeys = deviceCompositeKey.ExtractParameterValues();

			string animalIdentifier = string.Empty;
			// removing nlis_id or rf_id keys
			if (deviceKeys.ContainsKey("nlis_id"))
				animalIdentifier = Regex.Replace(deviceKeys["nlis_id"], "nlis_id", string.Empty);
			else if (deviceKeys.ContainsKey("rf_id"))
				animalIdentifier = Regex.Replace(deviceKeys["rf_id"], "rf_id", string.Empty);

			return animalIdentifier;

		}


		public static bool IsDigitsOnly(this string str)
		{
			foreach (var c in str)
			{
				if (c < '0' || c > '9')
					return false;
			}

			return true;
		}

		public static bool IsNlisId(this string str)
		{
			if (!str.Contains(" ") && str.Length == 16 && !str.All(char.IsDigit))
			{
				return true;
			}

			return false;
		}

		public static bool IsRfId(this string str)
		{
			if (str.Length != 16 || str.Substring(3, 1) != " ")
			{
				return false;
			}

			var rfidWithoutSpace = str.Replace(" ", "");
			if (rfidWithoutSpace.All(char.IsDigit))
			{
				return true;
			}

			return false;
		}

		public static bool ValidDeviceIdentifier(this string str)
		{
			if (str.IsNlisId() || str.IsRfId())
			{
				return true;
			}
			return false;
		}

		public static string ToEndpointKey(this string pathAndQuery)
		{
			int resourceCountInUri = 0; // HACK: this needs to be fixed asap
			if (string.IsNullOrEmpty(pathAndQuery))
				return string.Empty;

			// all our uris are like /api/version/resource1/{resource1Key}/resource2/{resource2Key}
			var elements = pathAndQuery.Split('?').First().Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
			var pathWithoutResourceId = new StringBuilder();

			for (int i = 0; i < elements.Count; i++)
			{
				// ignore route's resource paramaters
				if (elements[i].Contains("{") || elements[i].Contains("}")
				    || elements[i].Contains("(") || elements[i].Contains(")")
				    || Char.IsDigit(elements[i], 0))
					continue;

				pathWithoutResourceId = pathWithoutResourceId.Append(elements[i]).Append("/");
				resourceCountInUri++;
				if (resourceCountInUri >= 3)
					break;
			}
			return pathWithoutResourceId.ToString().ToLower();
		}
	}
}