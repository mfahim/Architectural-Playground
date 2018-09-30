using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using MicroServices.Animal.Api.Data.Domains.Animal;
using MicroServices.Animal.Api.Features.Animal.Controller;
using MicroServices.Animal.Api.Infrastructure.AuofacModule;

namespace MicroServices.Animal.Api.Data.Cassandra.Implementations
{
	internal class AnimalCFBCassandraProvider : AnimalCassandraFactory, IAnimalCFBCassandraProvider
	{
		private const string tableName = "carcaseproperties";
		private PreparedStatement _insertAnimalCFB;
		private PreparedStatement _selectAnimalCFB;
		private PreparedStatement _updateAnimalCFB;

		public AnimalCFBCassandraProvider(string keyspace, ILogger logger, IConsulService consulService) : base(keyspace,
			logger, consulService)
		{
		}

		public async Task InsertAnimalCfb(CfbInfo animalCFB, Dictionary<string, string> properties)
		{
			var convertedCFBItems = new List<cfbItem>();
			foreach (var props in properties)
				convertedCFBItems.Add(new cfbItem {name = props.Key, value = props.Value});
			var boundStatement = _insertAnimalCFB.Bind(animalCFB.KillInfoID, (sbyte) animalCFB.TypeId, convertedCFBItems);
			await Session.ExecuteAsync(boundStatement);
		}

		public async Task UpdateAnimalCfb(CfbInfo animalCFB, Dictionary<string, string> properties)
		{
			var convertedCFBItems = new List<cfbItem>();
			foreach (var props in properties)
				convertedCFBItems.Add(new cfbItem {name = props.Key, value = props.Value});
			var boundStatement = _updateAnimalCFB.Bind(convertedCFBItems, animalCFB.KillInfoID, (sbyte) animalCFB.TypeId);
			await Session.ExecuteAsync(boundStatement);
		}

		public async Task<Dictionary<string, string>> GetAnimalCfb(CfbInfo animalCFB)
		{
			var boundStatement = _selectAnimalCFB.Bind(animalCFB.KillInfoID, (sbyte) animalCFB.TypeId);
			var cfbPros = await Session.ExecuteAsync(boundStatement);
			if (cfbPros == null)
				throw new InvalidOperationException("Failed to find any CFB document by the required kill-key.");

			var cfbPayload = cfbPros.Select(p => p["payload"]).FirstOrDefault();
			var cfbKeyValueItems = cfbPayload as cfbItem[];
			var cfbProperties = new Dictionary<string, string>();
			foreach (var cfb in cfbKeyValueItems)
				cfbProperties.Add(cfb.name, cfb.value);
			return cfbProperties;
		}

		protected override async Task PrepareStatements()
		{
			_insertAnimalCFB =
				Session.Prepare(
					$"INSERT INTO {tableName} (killinfoid, documenttypeid, payload) VALUES(?, ?, ?);");

			_updateAnimalCFB =
				Session.Prepare($"Update {tableName} set payload = ? where killinfoid=? and documenttypeid=?;");

			_selectAnimalCFB =
				Session.Prepare($"Select payload FROM {tableName} where killinfoid=? and documenttypeid=?;");

			Session.UserDefinedTypes.Define(
				UdtMap.For<cfbItem>()
			);
		}

		private class cfbItem
		{
			public string name { get; set; }
			public string value { get; set; }
		}
	}
}