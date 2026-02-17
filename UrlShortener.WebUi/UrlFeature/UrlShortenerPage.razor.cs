using Microsoft.AspNetCore.Components.QuickGrid;

namespace UrlShortener.WebUi.UrlFeature;

public partial class UrlShortenerPage {
    private const int _pageSize = 20;

    private readonly UrlShortenerApiService _apiService;

    public GridItemsProvider<ShortUrlTableModel> ItemsProvider { get; set; }
    public PaginationState Pagination { get; set; } = new PaginationState { ItemsPerPage = _pageSize };
    public string UrlInput { get; set; } = "";

    public UrlShortenerPage(UrlShortenerApiService apiService) {
        _apiService = apiService;
        ItemsProvider = async req => {
            var page = req.StartIndex / _pageSize + 1;
            var result = await _apiService.GetUrlListAsync(page, _pageSize);
            if (result is null) {
                return GridItemsProviderResult.From(new List<ShortUrlTableModel>(), 0);
            }

            return GridItemsProviderResult.From(result.Items.Select(x => new ShortUrlTableModel(x)).ToList(), result.OverallCount);
        };
    }

    public async Task AddUrlAsync() {
        if (string.IsNullOrWhiteSpace(UrlInput)) {
            return;
        }
        await _apiService.ShortenUrlAsync(UrlInput);
        await Pagination.SetCurrentPageIndexAsync(0);
        UrlInput = "";
    }
}
