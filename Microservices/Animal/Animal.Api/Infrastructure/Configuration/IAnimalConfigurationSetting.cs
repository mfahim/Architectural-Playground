using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Caching.Memory;
using MicroServices.Animal.Api.Common.Cqrs;
using MicroServices.Animal.Api.Features.Animal.Cqrs.Messages.Commands;

namespace MicroServices.Animal.Api.Infrastructure.Configuration
{
	public interface ILogMessage
	{
		string ApplicationName { get; }
		string LogEventLevel { get; }
		string RequestId { get; }
		string MachineName { get; }
		object Request { get; }
		object Response { get; }
		string Message { get; }
		string InternalRequestId { get; }
		string UserId { get; }
		string ClientId { get; }
		string ClientIpAddress { get; }
		DateTime Time { get; }
		double? ProcessingTime { get; }
		string RequestUri { get; }
		string Exception { get; }
		string Endpoint { get; }
	}

	public interface IBusinessRule<TRequest, TResponse>
	{
		Task<RuleResultModel> Evaluate(TRequest request, TResponse queryResponse);
		string RuleId { get; }
	}

	// placeholder for future animal settings
	public interface IAnimalMicroServiceSetting : IConfigurationSetting
	{
	}

	public static class NlisEitherExtensions
	{
		public static DispatcherResponse<T> ToResult<T>(this Either<ExceptionResponse, T> @this) where T : BaseResponseType
			=> @this.Match(
				Right: data => new DispatcherResponse<T>(data),
				Left: error => new DispatcherResponse<T>(error));

		public static RestResult ToRestResult<T>(this Either<ExceptionResponse, T> @this,
			HttpStatusCode statusCode = HttpStatusCode.OK) where T : BaseResponseType
		{
			var responseResult = ToResult(@this);
			return responseResult.Succeeded ? CreateRestResultWhenSuccessful(responseResult, responseResult.Data, statusCode)
				: CreateRestResultWhenFailed(responseResult.Exception);
		}

		private static RestResult CreateRestResultWhenFailed(ExceptionResponse exception)
		{
			return new RestResult(RestContent.CreateWithGenericError(exception.ErrorMessage), exception.HttpStatusCode);
		}

		private static RestResult CreateRestResultWhenSuccessful<T>(DispatcherResponse<T> data, object value, HttpStatusCode statusCode) where T : BaseResponseType
		{
			var warnings = Enumerable.Empty<ValidationOutcome>();
			var infos = Enumerable.Empty<ValidationOutcome>();
			var errors = Enumerable.Empty<ValidationOutcome>();
			var ruleResults = data.Data.RuleResults;

			if (ruleResults != null)
			{
				warnings = ruleResults
					.Where(rl => rl.ValidationOutcome.OutcomeStatus.Equals(BusinessRule.OutcomeStatus.Warning.ToString()))
					.Select(p => p.ValidationOutcome).ToList();
				infos = ruleResults
					.Where(rl => rl.ValidationOutcome.OutcomeStatus.Equals(BusinessRule.OutcomeStatus.Info.ToString()))
					.Select(p => p.ValidationOutcome).ToList();

				errors = ruleResults
					.Where(rl => rl.ValidationOutcome.OutcomeStatus.Equals(BusinessRule.OutcomeStatus.Error.ToString()))
					.Select(p => p.ValidationOutcome).ToList();
			}

			var restContent = new RestContent(value,
				errors.Any() ? errors : null, warnings.Any() ? warnings : null, infos.Any() ? infos : null);

			return new RestResult(restContent, statusCode);
		}
	}

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

	public class BadRequestException : ApplicationException
	{
		public BadRequestException(string errorContent) : base(errorContent)
		{ }

		public static RestContent CreateRestContent(string errorContent)
		{
			if (string.IsNullOrEmpty(errorContent))
				return RestContent.CreateWithGenericError("Bad Request");

			return RestContent.CreateWithGenericError(errorContent);
		}
		public static HttpStatusCode HttpStatus() => HttpStatusCode.BadRequest;
	}

	public class MemoryCachingService : ICachingService
	{
		private readonly TimeSpan _defaultExpirationTimeSpan;
		private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks;
		private readonly IMemoryCache _cache;

		public MemoryCachingService(TimeSpan defaultExpirationTimeSpan)
		{
			this._defaultExpirationTimeSpan = defaultExpirationTimeSpan;
			this._locks = new ConcurrentDictionary<string, SemaphoreSlim>();
			this._cache = new MemoryCache(new MemoryCacheOptions());
		}

		public async Task<T> Get<T>(string key, TimeSpan? expireIn = null)
		{
			return await Get(key, expireIn ?? _defaultExpirationTimeSpan, () => Task.Run(() => default(T)));
		}

		public async Task<T> Get<T>(string key, Func<Task<T>> method)
		{
			return await Get(key, _defaultExpirationTimeSpan, method);
		}

		public async Task<T> Get<T>(string key, TimeSpan expireIn, Func<Task<T>> method)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				throw new ArgumentNullException(nameof(key));
			}
			var result = this._cache.Get(key);
			if (result != null)
				return (T)result;

			var keyLock = _locks.GetOrAdd(key, x => new SemaphoreSlim(1));
			await keyLock.WaitAsync();

			try
			{
				result = this._cache.Get(key);
				if (result != null)
					return (T)result;

				var cacheItemPolicy = new MemoryCacheEntryOptions() { AbsoluteExpiration = new DateTimeOffset(DateTime.Now.Add(expireIn)) };
				result = await method();
				if (result != null)
				{
					this._cache.Set(key, result, cacheItemPolicy);
				}
			}
			finally
			{
				keyLock.Release();
			}

			return (T)(result ?? default(T));
		}

		public void Clear(string key)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				throw new ArgumentNullException(nameof(key));
			}
			if (_cache.TryGetValue(key, out string value))
				this._cache.Remove(key);
		}

		public void Dispose()
		{
			this._cache.Dispose();
		}
	}
}