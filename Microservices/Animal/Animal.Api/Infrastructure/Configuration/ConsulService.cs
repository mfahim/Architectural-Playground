using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MicroServices.Animal.Api.Features.Animal.Controller;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MicroServices.Animal.Api.Infrastructure.Configuration
{
	public class ConsulService : IConsulService, IDisposable
	{
		private readonly TimeSpan _expirationTimeSpan;
		private readonly IMemoryCache _cache;
		private readonly IHttpClient _client;
		private readonly string _configRoot;
		private readonly string _baseUri;

		public ConsulService(IHttpClient client, string configRoot, TimeSpan configCacheExpirationTimeSpan, IPAddress consulIpAddress)
		{

			this._configRoot = configRoot;
			_expirationTimeSpan = configCacheExpirationTimeSpan;
			this._cache = new MemoryCache(new MemoryCacheOptions());
			this._client = client;
			this._baseUri = $"http://{consulIpAddress}:8500/v1";
		}

		public async Task<string> Get(string key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));
			// if it contains the key, just return
			if (CacheExtensions.TryGetValue(_cache, key, out string cachedValue))
			{
				return cachedValue;
			}

			var value = await GetOrCreate(key);
			if (value != null)
			{
				// the memory cache is threadsafe so we don't need to acquire the lock. the worst case
				// scenrio is we call the underlying consul service multiple times initially, but it would
				// be faster than getting the lock everytime
				CacheExtensions.Set<string>(_cache, key, value, new MemoryCacheEntryOptions() { AbsoluteExpiration = DateTime.Now.Add(_expirationTimeSpan) });
			}

			return value;
		}

		public async Task<int> GetInt(string key)
		{
			int val;
			var valString = await Get(key) ?? "0";
			if (!int.TryParse(valString, out val))
				val = 0;
			return val;
		}

		public async Task<bool> GetBool(string key)
		{
			var valString = await Get(key) ?? "";
			valString = valString.ToLower();

			if (valString == "true" || valString == "yes" || valString == "1")
				return true;
			if (valString == "false" || valString == "no" || valString == "0")
				return false;

			throw new ArgumentException("Invalid parameter in Consul settings.");
		}

		public async Task RegisterService(ServiceRegistrationRequest request)
		{
			var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

			await _client.PutAsync($"{_baseUri}/agent/service/register", content);
		}

		private async Task<string> GetOrCreate(string key, string defaultValue = null)
		{
			var response = await _client.GetStringAsync($"{_baseUri}/kv/{_configRoot}/{key}");
			if (!string.IsNullOrEmpty(response))
				return JToken.Parse(response).SelectToken("..Value").ToString().FromBase64();


			if (defaultValue != null)
				await _client.PutAsync($"{_baseUri}/kv/{_configRoot}/{key}", new StringContent(defaultValue));

			return defaultValue;
		}

		public async Task DeregisterService(string serviceId)
		{
			await _client.GetStringAsync($"{_baseUri}/agent/service/deregister/{serviceId}");
		}

		public void Dispose()
		{
			_cache?.Dispose();
		}
	}
}