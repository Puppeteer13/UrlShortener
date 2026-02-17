using UrlShortener.Shared;

namespace UrlShortener.WebUi.UrlFeature;

public class ShortUrlTableModel {
    public string ShortCode { get; set; } = "";
    public string FullUrl { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public long RedirectCount { get; set; }

    public ShortUrlTableModel(ShortenedUrlModel apiModel) {
        ShortCode = apiModel.ShortCode;
        FullUrl = apiModel.FullUrl;
        RedirectCount = apiModel.RedirectCount;
        CreatedAt = DateTimeOffset.FromUnixTimeSeconds(apiModel.CreatedAt).LocalDateTime;
    }
}
