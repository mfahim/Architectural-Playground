using System.Threading.Tasks;

namespace MicroServices.Animal.Api.Features.Animal.Controller
{
	public interface IConsulService
	{
		Task<string> Get(string key);
		Task<int> GetInt(string key);
		Task<bool> GetBool(string key);

		Task RegisterService(ServiceRegistrationRequest request);
		Task DeregisterService(string serviceId);
	}
}