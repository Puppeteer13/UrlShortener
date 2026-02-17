using Microsoft.EntityFrameworkCore;
using UrlShortener.Dal.Entities;

namespace UrlShortener.Dal;

public class AppDbContext : DbContext {
    public DbSet<ShrotenedUrl> ShortenedUrls { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
