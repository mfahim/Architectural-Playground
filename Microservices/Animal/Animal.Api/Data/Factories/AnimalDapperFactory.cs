using System.Data;
using System.Data.SqlClient;

namespace MicroServices.Animal.Api.Data.Factories
{
	public interface IDapperConnectionFactory
	{
		IDbConnection GetOpenConnection();
	}

	public class AnimalDapperFactory : IDapperConnectionFactory
	{
		private readonly string _connectionString;

		public AnimalDapperFactory(string connectionString)
		{
			_connectionString = connectionString;
		}

		public IDbConnection GetOpenConnection()
		{
			var connection = new SqlConnection(_connectionString);
			connection.Open();
			return connection;
		}
	}
}