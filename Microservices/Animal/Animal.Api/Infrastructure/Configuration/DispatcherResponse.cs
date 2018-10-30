using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Commands;

namespace MicroServices.Animal.Api.Infrastructure.Configuration
{
	public class DispatcherResponse<T> where T : BaseResponseType
	{
		public bool Succeeded { get; }
		public bool Failed => !Succeeded;

		public T Data { get; }
		public ExceptionResponse Exception { get; }

		internal DispatcherResponse(T data)
		{
			Succeeded = true;
			Data = data;
		}
		internal DispatcherResponse(ExceptionResponse exception) { Exception = exception; }
	}
}