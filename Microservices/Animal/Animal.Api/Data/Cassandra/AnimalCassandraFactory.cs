using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using MicroServices.Animal.Api.Features.Animal.Controller;
using MicroServices.Animal.Api.Infrastructure.AuofacModule;

namespace MicroServices.Animal.Api.Data.Cassandra
{
	internal abstract class AnimalCassandraFactory : IDisposable
	{
		private readonly ICluster cluster;
		private readonly IConsulService consulService;
		protected readonly ILogger Logger;
		protected readonly ISession Session;
		private bool isMappingConfigured;

		protected AnimalCassandraFactory(string keyspace, ILogger logger, IConsulService consulService)
		{
			var contactPoint = consulService.Get("Hosts/Cassandra/Host").GetAwaiter().GetResult();
			var contactPort = consulService.Get("Hosts/Cassandra/Port").GetAwaiter().GetResult();
			int portNumber;
			if (!int.TryParse(contactPort, out portNumber))
				throw new InvalidOperationException("Cassndra's port number is not valid.");

			var stopWatch = Stopwatch.StartNew();

			cluster = Cluster.Builder()
				.AddContactPoints(contactPoint.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)))
				.WithPort(portNumber)
				.WithCompression(CompressionType.Snappy)
				.Build();
			Session = cluster.Connect(keyspace);
			stopWatch.Stop();
			logger.Debug(null, $"CassandraProvider.ctor('{keyspace}') ⌚{stopWatch.ElapsedMilliseconds}ms").GetAwaiter()
				.GetResult();

			Logger = logger;
			this.consulService = consulService;

			Task.Run(PrepareStatements).GetAwaiter().GetResult();
		}

		public void Dispose()
		{
			Session.Dispose();
			cluster.Dispose();
		}

		protected abstract Task PrepareStatements();
	}
}