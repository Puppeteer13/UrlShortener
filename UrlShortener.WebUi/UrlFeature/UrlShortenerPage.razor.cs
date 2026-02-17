using UrlShortener.Shared;

namespace UrlShortener.WebUi.UrlFeature;

public partial class UrlShortenerPage {
    private const int _pageSize = 20;

    public IQueryable<ShortenedUrlModel> ShortUrls { get; set; } = new List<ShortenedUrlModel>().AsQueryable();

    protected override async Task OnInitializedAsync() {
        ShortUrls = (await UrlShortenerApiService.GetUrlListAsync(1, _pageSize))?.AsQueryable() ?? new List<ShortenedUrlModel>().AsQueryable();
        await base.OnInitializedAsync();
    }
}
