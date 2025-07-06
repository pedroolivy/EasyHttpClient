using System.Text.Json;

namespace EasyHttpClient
{
    public class ApiClientService<T> : IApiClientService<T>
    {
        private readonly HttpClient _httpClient;

        public ApiClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<List<T>> GetAsync(string url, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new List<T>();
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var data = JsonSerializer.Deserialize<List<T>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data ?? new List<T>();
        }
    }
}