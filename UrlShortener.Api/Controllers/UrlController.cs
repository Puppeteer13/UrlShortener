using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Dal;
using UrlShortener.Dal.Entities;
using UrlShortener.Shared;

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
    public async Task<IActionResult> ShortenUrl(ShortenUrlRequestModel request) {
        if (string.IsNullOrWhiteSpace(request.FullUrl))
            return BadRequest("URL is required.");

        if (!Uri.TryCreate(request.FullUrl, UriKind.Absolute, out var uriResult) ||
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

        var shortenedUrl = new ShortenedUrl {
            ShortCode = code,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            FullUrl = request.FullUrl
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

    [HttpGet("List")]
    public async Task<IActionResult> ListShortUrls([FromQuery] int pageNum, [FromQuery] int pageSize) {
        var list = await _dbContext.ShortenedUrls.AsNoTracking().OrderByDescending(x => x.CreatedAt).Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();
        var count = await _dbContext.ShortenedUrls.CountAsync();
        var result = new ShortenedUrlResponsePage {
            Items = list.Select(x => new ShortenedUrlModel {
                ShortCode = x.ShortCode,
                CreatedAt = x.CreatedAt,
                FullUrl = x.FullUrl,
                RedirectCount = x.RedirectCount
            }).ToList(),
            OverallCount = count
        };
        return Ok(result);
    }

    [HttpDelete("{code}")]
    public async Task<IActionResult> DeleteShortUrl(string code) {
        await _dbContext.ShortenedUrls.Where(x => x.ShortCode == code).ExecuteDeleteAsync();
        return NoContent();
    }

    [HttpPut("{code}")]
    public async Task<IActionResult> UpdateShortUrl(string code, ShortenedUrlModel model) {
        if (string.IsNullOrWhiteSpace(model.FullUrl))
            return BadRequest("URL is required.");

        if (!Uri.TryCreate(model.FullUrl, UriKind.Absolute, out var uriResult) ||
            (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)) {
            return BadRequest("Invalid URL format. Only HTTP/HTTPS are allowed.");
        }

        if (string.IsNullOrWhiteSpace(model.ShortCode) || model.ShortCode.Length != _codeLength || !model.ShortCode.All(x => _availableSymbols.Contains(x))) {
            return BadRequest("Invalid Short Code");
        }

        await _dbContext.ShortenedUrls
            .Where(x => x.ShortCode == code)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.ShortCode, model.ShortCode)
                .SetProperty(x => x.FullUrl, model.FullUrl));

        return NoContent();
    }
}
