namespace MicroServices.Animal.Api.Common.Cqrs
{
	public class ValidationOutcome
	{
		public string Code { get; set; }
		public string OutcomeStatus { get; set; }
		public string Message { get; set; }
		public string Description { get; set; }
#if DEBUG
		public string InnerException { get; set; }
		public string StackTrace { get; set; }
#endif
	}
}