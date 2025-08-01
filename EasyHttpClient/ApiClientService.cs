using System.Net.Http.Json;
using System.Text.Json;

namespace EasyHttpClient
{
    public class ApiClientService<T> : IApiClientService<T>
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<T>> GetAsync(string url, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new List<T>();
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var data = JsonSerializer.Deserialize<List<T>>(json, _jsonOptions);

            return data ?? new List<T>();
        }

        public async Task<T> GetByIdAsync(string url, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var data = JsonSerializer.Deserialize<T>(json, _jsonOptions);

            return data!;
        }

        public async Task<T> PostAsync(string url, T data, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(url, data, _jsonOptions, cancellationToken);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<T>(json, _jsonOptions);

            return result!;
        }

        public async Task<T> PutAsync(string url, T data, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync(url, data, _jsonOptions, cancellationToken);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<T>(json, _jsonOptions);

            return result!;
        }

        public async Task<bool> DeleteAsync(string url, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync(url, cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
}