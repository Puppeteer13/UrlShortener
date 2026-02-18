using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using UrlShortener.WebUi;
using UrlShortener.WebUi.UrlFeature;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("Api", client => {
    client.BaseAddress = new Uri(AppSettings.ApiBaseUrl);
});

builder.Services.AddFluentUIComponents(options => {
    options.ValidateClassNames = false;
});

builder.Services.AddSingleton<UrlShortenerApiService>();

await builder.Build().RunAsync();
