using MongoDB.Driver;
using UrlShortener.WebApi.Models;

namespace UrlShortener.WebApi.Mongo
{
    /// <summary>
    /// Default implementation of a custom seeder.
    /// </summary>
    public class MongoSeeder : IDatabaseSeeder
    {
        protected readonly IMongoDatabase _database;

        public MongoSeeder(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task SeedAsync()
        {
            var collectionCursor = await _database.ListCollectionsAsync();
            var collections = await collectionCursor.ToListAsync();
            if (collections.Any())
            {
                return;
            }
            await CustomSeedAsync();
        }

        protected virtual async Task CustomSeedAsync()
        {
            var shortenedUrls = new List<ShortenedUrl>
            {
                new ShortenedUrl
                {
                    Id = Guid.NewGuid(),
                    LongUrl = "https://www.linkedin.com/in/optiklab/",
                    Code = "ABC",
                    ShortUrl = $"https://tactica.xyz/ABC",
                    CreatedOnUtc = DateTime.UtcNow
                },
                new ShortenedUrl
                {
                    Id = Guid.NewGuid(),
                    LongUrl = "https://optiklab.github.io/",
                    Code = "XYZ",
                    ShortUrl = $"https://tactica.xyz/XYZ",
                    CreatedOnUtc = DateTime.UtcNow
                }
            };

            await Task.WhenAll(shortenedUrls.Select(x => Collection.InsertOneAsync(x)));
        }

        private IMongoCollection<ShortenedUrl> Collection
            => _database.GetCollection<ShortenedUrl>("ShortenedUrls");
    }
}