namespace MicroServices.Animal.Api.Data.Domains.ProgramsStatus
{
	public class ProgramStatusAggregateEntity
	{
		public Program Program { get; set; }
		public ProgramStatus Statuses { get; set; }
	}
}