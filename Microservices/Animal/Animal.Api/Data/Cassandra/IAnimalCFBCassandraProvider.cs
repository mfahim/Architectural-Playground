using System.Collections.Generic;
using System.Threading.Tasks;
using MicroServices.Animal.Api.Data.Domains.Animal;

namespace MicroServices.Animal.Api.Data.Cassandra
{
	public interface IAnimalCFBCassandraProvider
	{
		Task InsertAnimalCfb(CfbInfo animalCFB, Dictionary<string, string> properties);
		Task UpdateAnimalCfb(CfbInfo animalCFB, Dictionary<string, string> properties);
		Task<Dictionary<string, string>> GetAnimalCfb(CfbInfo animalCFB);
	}
}