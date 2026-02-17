using System.Net.Http.Json;
using UrlShortener.Shared;

namespace UrlShortener.WebUi.UrlShortener;

public class UrlShortenerApiService {
    private static readonly HttpClient _httpClient;

    static UrlShortenerApiService() {
        _httpClient = new HttpClient(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(5) });
    }

    public async Task<ShortenedUrlModel?> ShortenUrlAsync(string url) {
        var encodedUrl = Uri.EscapeDataString(url);
        var response = await _httpClient.PostAsync($"Url/Shorten?url={encodedUrl}", null);

        if (!response.IsSuccessStatusCode) {
            // Handle error (log it, throw exception, etc.)
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error shortening URL: {errorContent}");
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ShortenedUrlModel>();
    }
}
