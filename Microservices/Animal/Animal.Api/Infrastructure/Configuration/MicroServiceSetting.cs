using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;

namespace MicroServices.Animal.Api.Infrastructure.Configuration
{
	public class MicroServiceSetting : IAnimalMicroServiceSetting
	{
		public string ApplicationName { get; set; }
		public string ConsulIpAddress { get; set; }
	}
}