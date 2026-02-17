using Microsoft.EntityFrameworkCore;
using UrlShortener.Dal;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseMySql("Server=localhost;Database=urlshortener;User=root;Password=my-secret-pw;Port=3306;", new MariaDbServerVersion(new Version(12, 2)));
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
