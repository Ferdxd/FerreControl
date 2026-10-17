using System.Net.Http;
using System.Net.Http.Json;

namespace FerreControl.Desktop.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(
                    "https://localhost:7217/")
            };
        }


        public async Task<T?> GetAsync<T>(string endpoint)
        {
            return await _httpClient.GetFromJsonAsync<T>(endpoint);
        }


        public async Task<HttpResponseMessage>PostAsync<T>(string endpoint,T data)
        {
            return await _httpClient.PostAsJsonAsync(endpoint, data);
        }


        public async Task<HttpResponseMessage>PutAsync<T>(string endpoint,T data)
        {
            return await _httpClient.PutAsJsonAsync(endpoint, data);
        }
    }
}