using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MicroServices.Animal.Api.Infrastructure.Configuration;
using MicroServices.Animal.Api.Infrastructure.Configuration.Interfaces;
using Newtonsoft.Json;

namespace MicroServices.Animal.Api.Infrastructure.AuofacModule
{
	public class StandardHttpClient : IHttpClient
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private HttpClient _client;

		public StandardHttpClient(IHttpContextAccessor httpContextAccessor)
		{
			_client = new HttpClient();
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
		{
			var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

			var response = await SendAsync(requestMessage);

			return await response.Content.ReadAsStringAsync();
		}

		private async Task<HttpResponseMessage> DoPostPutAsync<T>(HttpMethod method, string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
		{
			if (method != HttpMethod.Post && method != HttpMethod.Put)
			{
				throw new ArgumentException("Value must be either post or put.", nameof(method));
			}

			// a new StringContent must be created for each retry
			// as it is disposed after each call

			var requestMessage = new HttpRequestMessage(method, uri);

			requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");

			await SendAsync(requestMessage, authorizationToken, requestId);

			return await _client.SendAsync(requestMessage);

		}


		public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
		{
			return await DoPostPutAsync(HttpMethod.Post, uri, item, authorizationToken, requestId, authorizationMethod);
		}

		public async Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
		{
			return await DoPostPutAsync(HttpMethod.Put, uri, item, authorizationToken, requestId, authorizationMethod);
		}

		public async Task<HttpResponseMessage> PatchAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
		{
			return await DoPostPutAsync(new HttpMethod("PATCH"), uri, item, authorizationToken, requestId, authorizationMethod);
		}

		public async Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
		{
			var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

			return await SendAsync(requestMessage, authorizationToken, requestId);
		}


		private void SetAuthorizationHeader(HttpRequestMessage requestMessage)
		{
			var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
			if (!string.IsNullOrEmpty(authorizationHeader))
			{
				requestMessage.Headers.Add("Authorization", new List<string>() { authorizationHeader });
			}
		}

		public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer")
		{
			SetAuthorizationHeader(requestMessage);

			if (authorizationToken != null)
			{
				requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
			}

			if (requestId != null)
			{
				requestMessage.Headers.Add("x-requestid", requestId);
			}

			try
			{
				var response = await _client.SendAsync(requestMessage);

				return response;

			}
			catch (Exception ex)
			{
				throw new HttpRequestException($"{requestMessage.Method} request to '{requestMessage.RequestUri}' failed", ex);
			}
		}

	}
}