namespace UrlShortener.Shared;

public class ShortenedUrlResponsePage {
    public List<ShortenedUrlModel> Items { get; set; } = [];
    public int OverallCount { get; set; }
}
