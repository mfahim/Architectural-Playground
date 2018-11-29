using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;

namespace MicroServices.Animal.Api.Infrastructure.Configuration
{
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
			var result = CacheExtensions.Get(this._cache, key);
			if (result != null)
				return (T)result;

			var keyLock = _locks.GetOrAdd(key, x => new SemaphoreSlim(1));
			await keyLock.WaitAsync();

			try
			{
				result = CacheExtensions.Get(this._cache, key);
				if (result != null)
					return (T)result;

				var cacheItemPolicy = new MemoryCacheEntryOptions() { AbsoluteExpiration = new DateTimeOffset(DateTime.Now.Add(expireIn)) };
				result = await method();
				if (result != null)
				{
					CacheExtensions.Set(this._cache, key, result, cacheItemPolicy);
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
			if (CacheExtensions.TryGetValue(_cache, key, out string value))
				this._cache.Remove(key);
		}

		public void Dispose()
		{
			this._cache.Dispose();
		}
	}
}