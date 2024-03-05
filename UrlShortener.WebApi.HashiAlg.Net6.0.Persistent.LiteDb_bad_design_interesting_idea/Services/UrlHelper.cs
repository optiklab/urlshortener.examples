using HashidsNet;
using UrlShortener.WebApi.Hashi.Services.Interface;

namespace UrlShortener.WebApi.Hashi.Services
{
    public class UrlHelper : IUrlHelper
    {
        private Hashids _hashIds;
        public UrlHelper()
        {
            _hashIds = new Hashids("This is my shortener", 6);
        }

        /// <summary>
        /// Gets Shortened URL based on our Lite DB Row Id.
        /// </summary>
        public string GetShortUrl(int id)
        {
            return _hashIds.Encode(id);
        }

        /// <summary>
        /// Retrieves mapping of Shortened URL to the Id (that we use in our Db).
        /// </summary>
        public int GetId(string shortUrl)
        {
            var decodedId = _hashIds.Decode(shortUrl);
            return decodedId[0];
        }
    }
}
