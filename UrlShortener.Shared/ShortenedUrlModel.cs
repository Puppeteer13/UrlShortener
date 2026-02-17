namespace UrlShortener.Shared;

public class ShortenedUrlModel {
    public string ShortCode { get; set; } = "";
    public string FullUrl { get; set; } = "";
    public long CreatedAt { get; set; }
    public long RedirectCount { get; set; }
}
