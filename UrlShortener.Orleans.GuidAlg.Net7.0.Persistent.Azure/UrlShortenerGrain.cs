using Orleans.Runtime;

namespace UrlShortener.Orleans.WebApp
{
    // Grains are virtual actors, they can have identity, behavior and state.
    // State can be changed but it will always be stored in memory while
    // it is active, improving performance.
    public class UrlShortenerGrain : Grain, IUrlShortenerGrain
    {
        private IPersistentState<KeyValuePair<string, string>> _cache;

        public UrlShortenerGrain(
            [PersistentState(
                stateName: "url",
                storageName: "urls")]
                IPersistentState<KeyValuePair<string, string>> cache
            )
        {
            _cache = cache;
        }

        // Set URL redirect for the shortened Url
        public async Task SetUrl(string shortenedRouteSegment, string fullUrl)
        {
            _cache.State = new KeyValuePair<string, string>(shortenedRouteSegment, fullUrl);

            await _cache.WriteStateAsync();
        }

        // Get redirect url.
        public async Task<string> GetUrl()
        {
            return await Task.FromResult(_cache.State.Value);
        }
    }
}
