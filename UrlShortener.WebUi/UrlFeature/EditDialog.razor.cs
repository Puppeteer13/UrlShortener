using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using UrlShortener.Shared;

namespace UrlShortener.WebUi.UrlFeature;

public partial class EditDialog : IDialogContentComponent<ShortenedUrlModel> {
    [Parameter]
    public ShortenedUrlModel Content { get; set; } = null!;

    [CascadingParameter]
    public FluentDialog? Dialog { get; set; }
}
