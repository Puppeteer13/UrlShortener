using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Dal;
using UrlShortener.Dal.Entities;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UrlController : ControllerBase {
    private const string _availableSymbols = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int _codeLength = 10;
    private readonly AppDbContext _dbContext;

    public UrlController(AppDbContext dbContext) {
        _dbContext = dbContext;
    }

    [HttpPost("Shorten")]
    public async Task<IActionResult> ShortenUrl([FromQuery]string url) {
        if (string.IsNullOrWhiteSpace(url))
            return BadRequest("URL is required.");

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) ||
            (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)) {
            return BadRequest("Invalid URL format. Only HTTP/HTTPS are allowed.");
        }

        string code;
        do {
            var codeChars = new char[_codeLength];
            for (int i = 0; i < _codeLength; i++) {
                codeChars[i] = _availableSymbols[Random.Shared.Next(_availableSymbols.Length)];
            }
            code = new string(codeChars);
        } while (await _dbContext.ShortenedUrls.AnyAsync(x => x.ShortCode == code));

        var shortenedUrl = new ShrotenedUrl {
            ShortCode = code,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            FullUrl = url
        };

        _dbContext.Add(shortenedUrl);
        await _dbContext.SaveChangesAsync();

        return Ok(shortenedUrl);
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> RedirectToResult(string code) {
        var fullUrl = await _dbContext.ShortenedUrls
            .AsNoTracking()
            .Where(x => x.ShortCode == code)
            .Select(x => x.FullUrl)
            .FirstOrDefaultAsync();

        if (fullUrl is null) {
            return BadRequest("Url not found");
        }

        await _dbContext.ShortenedUrls
            .Where(x => x.ShortCode == code)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.RedirectCount, x => x.RedirectCount + 1));

        return Redirect(fullUrl);
    }
}
