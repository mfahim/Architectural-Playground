using System.Collections.Generic;

namespace MicroServices.Animal.Api.Features.Animal.Controller
{
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
}