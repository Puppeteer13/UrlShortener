using Microsoft.FluentUI.AspNetCore.Components;
using GridItemsProviderResult = Microsoft.AspNetCore.Components.QuickGrid.GridItemsProviderResult;
using PaginationState = Microsoft.AspNetCore.Components.QuickGrid.PaginationState;

namespace UrlShortener.WebUi.UrlFeature;

public partial class UrlShortenerPage {
    private const int _pageSize = 20;

    private readonly UrlShortenerApiService _apiService;
    private readonly IToastService _toastService;

    public Microsoft.AspNetCore.Components.QuickGrid.GridItemsProvider<ShortUrlTableModel> ItemsProvider { get; set; }
    public PaginationState Pagination { get; set; } = new PaginationState { ItemsPerPage = _pageSize };
    public string UrlInput { get; set; } = "";

    public UrlShortenerPage(UrlShortenerApiService apiService, IToastService toastService) {
        _apiService = apiService;
        _toastService = toastService;
        ItemsProvider = async req => {
            var page = req.StartIndex / _pageSize + 1;
            try {
                var result = await _apiService.GetUrlListAsync(page, _pageSize);
                if (result is null) {
                    return GridItemsProviderResult.From(new List<ShortUrlTableModel>(), 0);
                }

                return GridItemsProviderResult.From(result.Items.Select(x => new ShortUrlTableModel(x)).ToList(), result.OverallCount);
            } catch (Exception ex) {
                _toastService.ShowError(ex.Message);
                return GridItemsProviderResult.From(new List<ShortUrlTableModel>(), 0);
            }
        };
    }

    public async Task AddUrlAsync() {
        if (string.IsNullOrWhiteSpace(UrlInput)) {
            _toastService.ShowError("Url cannot be empty");
            return;
        }
        try {
            await _apiService.ShortenUrlAsync(UrlInput);
        } catch (Exception ex) {
            _toastService.ShowError(ex.Message);
            return;
        }
        await Pagination.SetCurrentPageIndexAsync(0);
        UrlInput = "";
    }

    public async Task DeleteUrlAsync(string code) {
        try {
            await _apiService.DeleteShortUrlAsync(code);
        } catch (Exception ex) {
            _toastService.ShowError(ex.Message);
            return;
        }
        await Pagination.SetCurrentPageIndexAsync(0);
    }
}
