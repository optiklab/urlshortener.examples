using Orleans.Runtime;

namespace UrlShortener.Orleans.WebApp
{
    // Grains are virtual actors, they can have identity, behavior and state.
    // State can be changed but it will always be stored in memory while
    // it is active, improving performance.
    public class UrlShortenerGrain : Grain, IUrlShortenerGrain
    {
        private readonly ILogger<UrlShortenerGrain> _logger;

        private readonly IPersistentState<KeyValuePair<string, string>> _cache;

        public UrlShortenerGrain(
            [PersistentState(
                stateName: "url",
                storageName: "urls")]
                IPersistentState<KeyValuePair<string, string>> cache,
                ILogger<UrlShortenerGrain> logger
            )
        {
            _cache = cache;
            _logger = logger;
        }

        // Set URL redirect for the shortened Url
        public async Task SetUrl(string shortenedRouteSegment, string fullUrl)
        {
            _cache.State = new KeyValuePair<string, string>(shortenedRouteSegment, fullUrl);

            //_cache.Etag
            //_cache.RecordExists

            _logger.LogInformation($"{shortenedRouteSegment} mapped to {fullUrl}.");

            await _cache.WriteStateAsync();
        }

        // Get redirect url.
        public async Task<string> GetUrl()
        {
            // await _cache.ReadStateAsync(); // Called automatically when grain is activated.

            _logger.LogInformation($"Called {_cache.State.Value} by {_cache.State.Key}. Etag {_cache.Etag}.");

            return await Task.FromResult(_cache.State.Value);
        }
    }
}
