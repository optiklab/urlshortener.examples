using UrlShortener.WebApi.Hashi.Model;

namespace UrlShortener.WebApi.Hashi.Services.Interface
{
    public interface IApplicationDbContext
    {
        /// <summary>
        /// Adds URL into database.
        /// </summary>
        /// <param name="urlData">URL</param>
        /// <returns>Row database id.</returns>
        int AddUrl(ShortenedUrl urlData);

        /// <summary>
        /// Gets URL from the database by Row Id.
        /// </summary>
        /// <param name="id">Int32 row id in the database. That is the LIMIT of this application due to such a short number of possible records.</param>
        /// <returns>URL</returns>
        ShortenedUrl GetUrl(int id);

        /// <summary>
        /// Checks for existence in the database by simple search query in DB.
        /// </summary>
        /// <param name="longUrl">URL</param>
        /// <returns>Yes or No.</returns>
        bool CheckIfUrlExists(string longUrl);
    }
}
