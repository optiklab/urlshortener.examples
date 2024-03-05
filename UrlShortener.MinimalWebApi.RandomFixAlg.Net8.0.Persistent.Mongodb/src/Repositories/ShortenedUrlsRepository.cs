using MongoDB.Driver;
using MongoDB.Driver.Linq;
using UrlShortener.WebApi.Models;

namespace UrlShortener.WebApi.Repositories
{
    public class ShortenedUrlsRepository : IShortenedUrlsRepository
    {
        private readonly IMongoDatabase _database;

        public ShortenedUrlsRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task AddAsync(ShortenedUrl url)
            => await Collection.InsertOneAsync(url);

        public async Task<IEnumerable<ShortenedUrl>> BrowseAsync()
            => await Collection
                .AsQueryable()
                .ToListAsync();

        public async Task<bool> AnyAsync(string code)
            => await Collection
                .AsQueryable()
                .AnyAsync(x => x.Code == code);

        public async Task<ShortenedUrl> GetAsync(string code)
            => await Collection
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Code == code);

        private IMongoCollection<ShortenedUrl> Collection
            => _database.GetCollection<ShortenedUrl>("ShortenedUrls");
    }
}