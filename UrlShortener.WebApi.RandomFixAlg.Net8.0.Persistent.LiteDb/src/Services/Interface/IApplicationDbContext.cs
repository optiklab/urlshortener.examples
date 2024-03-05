using UrlShortener.WebApi.Model;

namespace UrlShortener.WebApi.Services.Interface
{
    public interface IApplicationDbContext
    {
        /// <summary>
        /// Adds URL into database.
        /// </summary>
        /// <param name="urlData">URL</param>
        /// <returns>Row database id.</returns>
        Guid AddUrl(ShortenedUrl urlData);

        /// <summary>
        /// Gets URL from the database by Row Id.
        /// </summary>
        /// <param name="code">Code of previousl shortened URL.</param>
        /// <returns>URL</returns>
        ShortenedUrl GetUrl(string code);

        /// <summary>
        /// Checks for existence of the code in the database by simple search query in DB.
        /// </summary>
        /// <param name="code">code</param>
        /// <returns>Yes or No.</returns>
        bool CheckIfCodeExists(string code);

        /// <summary>
        /// Retrieves all existing shortenings.
        /// </summary>
        /// <returns></returns>
        List<ShortenedUrl> GetAll();
    }
}
