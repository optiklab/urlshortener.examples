namespace UrlShortener.Orleans.WebApp
{
    // Grains are virtual actors, they can have identity, behavior and state.
    // State can be changed but it will always be stored in memory while
    // it is active, improving performance.
    public class UrlShortenerGrain : Grain, IUrlShortenerGrain
    {
        private KeyValuePair<string, string> _cache;

        // Set URL redirect for the shortened Url
        public  Task SetUrl(string shortenedRouteSegment, string fullUrl)
        {
            _cache = new KeyValuePair<string, string>(shortenedRouteSegment, fullUrl);

            return Task.CompletedTask;
        }

        // Get redirect url.
        public Task<string> GetUrl()
        {
            return Task.FromResult(_cache.Value);
        }
    }
}
