using System.Net.Http.Json;
using UrlShortener.Shared;

namespace UrlShortener.WebUi.UrlShortener;

public class UrlShortenerApiService {
    private static readonly HttpClient _httpClient;

    static UrlShortenerApiService() {
        _httpClient = new HttpClient(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(5) });
    }

    public static async Task<List<ShortenedUrlModel>?> GetUrlListAsync(int pageNum, int pageSize) {
        var response = await _httpClient.GetAsync($"Url/List?pageNum={pageNum}&pageSize={pageSize}");

        return await response.Content.ReadFromJsonAsync<List<ShortenedUrlModel>>();
    }

    public static async Task<ShortenedUrlModel?> ShortenUrlAsync(string url) {
        var encodedUrl = Uri.EscapeDataString(url);
        var response = await _httpClient.GetAsync($"Url/Shorten?url={encodedUrl}");

        if (!response.IsSuccessStatusCode) {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error shortening URL: {errorContent}");
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ShortenedUrlModel>();
    }
}
