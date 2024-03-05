namespace UrlShortener.Orleans.WebApp
{
    // Options for Base:
    // - IGrainWithGuidKey
    // - IGrainWithIntegerKey
    // - IGrainWithGuidCompoundKey
    // - IGrainWithIntegerCompoundKeys
    // - IGrainWithStringKey
    public interface IUrlShortenerGrain : IGrainWithStringKey
    {
        // Set URL redirect for the shortened Url
        Task SetUrl(string shortenedRouteSegment, string fullUrl);

        // Get redirect url.
        Task<string> GetUrl();
    }
}
