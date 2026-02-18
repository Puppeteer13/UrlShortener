using Microsoft.FluentUI.AspNetCore.Components;
using UrlShortener.Shared;
using GridItemsProviderResult = Microsoft.AspNetCore.Components.QuickGrid.GridItemsProviderResult;
using PaginationState = Microsoft.AspNetCore.Components.QuickGrid.PaginationState;

namespace UrlShortener.WebUi.UrlFeature;

public partial class UrlShortenerPage {
    private const int _pageSize = 20;

    private readonly UrlShortenerApiService _apiService;
    private readonly IToastService _toastService;
    private readonly IDialogService _dialogService;

    public Microsoft.AspNetCore.Components.QuickGrid.GridItemsProvider<ShortUrlTableModel> ItemsProvider { get; set; }
    public PaginationState Pagination { get; set; } = new PaginationState { ItemsPerPage = _pageSize };
    public string UrlInput { get; set; } = "";

    public UrlShortenerPage(UrlShortenerApiService apiService, IToastService toastService, IDialogService dialogService) {
        _apiService = apiService;
        _toastService = toastService;
        _dialogService = dialogService;
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
        _dialogService = dialogService;

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

    public async Task EditShortUrl(ShortUrlTableModel model) {
        var apiModel = new ShortenedUrlModel {
            ShortCode = model.ShortCode,
            FullUrl = model.FullUrl,
            RedirectCount = model.RedirectCount,
            CreatedAt = new DateTimeOffset(model.CreatedAt).ToUnixTimeSeconds()
        };

        var dialogRef = await _dialogService.ShowDialogAsync<EditDialog>(apiModel, new DialogParameters { 
            Title = "Edit Shortened Url", 
            PrimaryAction = "Confirm", 
            SecondaryAction = "Cancel"
        });

        var result = await dialogRef.Result;
        if (result.Cancelled || result?.Data is null || result?.Data is not ShortenedUrlModel newModel) {
            return;
        }

        try {
            await _apiService.UpdateShortUrlAsync(model.ShortCode, newModel);
        } catch (Exception ex) {
            _toastService.ShowError(ex.Message);
            return;
        }
        await Pagination.SetCurrentPageIndexAsync(0);
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
