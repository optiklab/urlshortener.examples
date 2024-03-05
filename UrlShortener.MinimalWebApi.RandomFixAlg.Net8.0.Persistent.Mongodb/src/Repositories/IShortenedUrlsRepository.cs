using UrlShortener.WebApi.Models;

namespace UrlShortener.WebApi.Repositories
{
    public interface IShortenedUrlsRepository
    {
        Task<bool> AnyAsync(string code);
        Task<ShortenedUrl> GetAsync(string code);
        Task<IEnumerable<ShortenedUrl>> BrowseAsync();
        Task AddAsync(ShortenedUrl category);
    }
}
