using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Dal.Entities;

namespace UrlShortener.Dal.Config;

public class ShortenedUrlConfig : IEntityTypeConfiguration<ShortenedUrl> {
    public void Configure(EntityTypeBuilder<ShortenedUrl> builder) {
        builder.HasKey(x => x.ShortCode);

        builder.Property(x => x.ShortCode).HasMaxLength(10);
    }
}
