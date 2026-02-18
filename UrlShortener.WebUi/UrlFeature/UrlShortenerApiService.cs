using System.Net.Http.Json;
using UrlShortener.Shared;

namespace UrlShortener.WebUi.UrlFeature;

public class UrlShortenerApiService {
    private readonly IHttpClientFactory _httpClientFactory;

    public UrlShortenerApiService(IHttpClientFactory factory) {
        _httpClientFactory = factory;
    }

    public async Task<ShortenedUrlResponsePage?> GetUrlListAsync(int pageNum, int pageSize) {
        var client = _httpClientFactory.CreateClient("Api");
        var response = await client.GetAsync($"Url/List?pageNum={pageNum}&pageSize={pageSize}");

        if (!response.IsSuccessStatusCode) {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception(errorContent);
        }

        return await response.Content.ReadFromJsonAsync<ShortenedUrlResponsePage>();
    }

    public async Task<ShortenedUrlModel?> ShortenUrlAsync(string url) {
        var client = _httpClientFactory.CreateClient("Api");
        var request = new ShortenUrlRequestModel { FullUrl = url };
        var response = await client.PostAsJsonAsync($"Url/Shorten", request);

        if (!response.IsSuccessStatusCode) {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception(errorContent);
        }

        return await response.Content.ReadFromJsonAsync<ShortenedUrlModel>();
    }

    public async Task DeleteShortUrlAsync(string code) {
        var client = _httpClientFactory.CreateClient("Api");
        var response = await client.DeleteAsync($"Url/{code}");

        if (!response.IsSuccessStatusCode) {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception(errorContent);
        }
    }
}
