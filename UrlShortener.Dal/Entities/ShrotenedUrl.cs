namespace UrlShortener.Dal.Entities;

public class ShrotenedUrl {
    public string ShortCode { get; set; } = "";
    public string FullUrl { get; set; } = "";
    public long CreatedAt { get; set; }
    public long RedirectCount { get; set; }
}
