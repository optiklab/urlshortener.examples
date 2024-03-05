using Microsoft.EntityFrameworkCore;
using UrlShortener.WebApi.Models;
using UrlShortener.WebApi.Services;

namespace UrlShortener.WebApi
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        { }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortenedUrl>(builder =>
            {
                builder.Property(s => s.Code).HasMaxLength(UrlShorteningService.NumberOfCharsInShortLink);

                builder.HasIndex(s => s.Code).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
