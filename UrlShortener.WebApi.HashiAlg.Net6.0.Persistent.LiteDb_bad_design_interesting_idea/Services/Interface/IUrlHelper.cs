namespace UrlShortener.WebApi.Hashi.Services.Interface
{
    public interface IUrlHelper
    {
        /// <summary>
        /// Gets Shortened URL based on our Lite DB Row Id.
        /// </summary>
        string GetShortUrl(int id);

        /// <summary>
        /// Retrieves mapping of Shortened URL to the Id (that we use in our Db).
        /// </summary>
        int GetId(string shortUrl);
    }
}
