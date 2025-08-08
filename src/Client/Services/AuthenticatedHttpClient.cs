using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Client.Services
{
    public class AuthenticatedHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public AuthenticatedHttpClient(HttpClient httpClient, IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        private async Task EnsureAuthHeaderAsync()
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            await EnsureAuthHeaderAsync();
            return await _httpClient.GetAsync(requestUri);
        }

        public async Task<T?> GetFromJsonAsync<T>(string requestUri)
        {
            await EnsureAuthHeaderAsync();
            return await _httpClient.GetFromJsonAsync<T>(requestUri);
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value)
        {
            await EnsureAuthHeaderAsync();
            return await _httpClient.PostAsJsonAsync(requestUri, value);
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent? content)
        {
            await EnsureAuthHeaderAsync();
            return await _httpClient.PostAsync(requestUri, content);
        }

        public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value)
        {
            await EnsureAuthHeaderAsync();
            return await _httpClient.PutAsJsonAsync(requestUri, value);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            await EnsureAuthHeaderAsync();
            return await _httpClient.DeleteAsync(requestUri);
        }
    }
}
