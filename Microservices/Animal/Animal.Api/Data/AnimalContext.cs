using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MicroServices.Animal.Api.Data.Configuration;
using MicroServices.Animal.Api.Data.Domains.Animal;
using MicroServices.Animal.Api.Data.Domains.Device;
using MicroServices.Animal.Api.Data.Domains.ProgramsStatus;
using MicroServices.Animal.Api.Data.Domains.Property;
using MicroServices.Animal.Api.Data.Domains.Reference;

namespace MicroServices.Animal.Api.Data
{
	public partial class AnimalContext : DbContext
	{
		private IDbContextTransaction _currentTransaction;

		public AnimalContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<Domains.Animal.Animal> Animals { get; set; }
		public DbSet<ProgramStatus> ProgramStatuses { get; set; }
		public DbSet<Device> Devices { get; set; }
		public DbSet<DeviceAssignment> DeviceAssignments { get; set; }
		public DbSet<AnimalCurrentState> AnimalCurrentState { get; set; }
		public DbSet<PropertyAnimalStatusRule> PropertyAnimalStatusRules { get; set; }
		public DbSet<ReachableProperty> ReachableProperties { get; set; }
		public DbSet<CfbInfo> CfbInfos { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new AnimalConfiguration());
			modelBuilder.ApplyConfiguration(new AnimalMovementHistoryConfiguration());
			modelBuilder.ApplyConfiguration(new AnimalAuditConfiguration());
			modelBuilder.ApplyConfiguration(new AnimalCurrentStateAuditConfiguration());

			modelBuilder.ApplyConfiguration(new DeviceConfiguration());
			modelBuilder.ApplyConfiguration(new DeviceAssignmentConfiguration());
			modelBuilder.ApplyConfiguration(new DeviceDefinitionConfiguration());

			modelBuilder.ApplyConfiguration(new KillConfiguration());

			modelBuilder.ApplyConfiguration(new ProgramStatusSpeciesConfiguration());

			// todo: create config if it has some special configs
			modelBuilder.Entity<BusinessRule>();
			modelBuilder.Entity<ReachableProperty>();
		}

		public async Task<int> SaveChangesAsync(long? requestId)
		{
			var result = 0;
			if (requestId == null)
			{
				result = await SaveChangesAsync();
			}
			else
			{
				var connection = Database.GetDbConnection();
				// check before openning the connection
				var needToManageConnection = connection.State != ConnectionState.Open;
				if (needToManageConnection)
					connection.Open();
				SetRequestContext(requestId);
				result = await SaveChangesAsync();
				if (needToManageConnection)
					connection.Close();
			}
			return result;
		}

		[DbFunction("SetRequestContext", Schema = "dbo")]
		public int SetRequestContext(long? requestId)
		{
			// attach RequestId to dbContext for audit tables (AuditRequestId, ..) 
			var requestIdParameter = new SqlParameter("@requestId", requestId);
			return Database.ExecuteSqlCommand("dbo.SetRequestContext @requestId", requestIdParameter);
		}

		public void BeginTransaction()
		{
			if (_currentTransaction != null)
				return;

			_currentTransaction = Database.BeginTransaction(IsolationLevel.ReadCommitted);
		}

		public async Task CommitTransactionAsync()
		{
			try
			{
				await SaveChangesAsync();

				_currentTransaction?.Commit();
			}
			catch
			{
				RollbackTransaction();
				throw;
			}
			finally
			{
				if (_currentTransaction != null)
				{
					_currentTransaction.Dispose();
					_currentTransaction = null;
				}
			}
		}

		public void RollbackTransaction()
		{
			try
			{
				_currentTransaction?.Rollback();
			}
			finally
			{
				if (_currentTransaction != null)
				{
					_currentTransaction.Dispose();
					_currentTransaction = null;
				}
			}
		}
	}
}