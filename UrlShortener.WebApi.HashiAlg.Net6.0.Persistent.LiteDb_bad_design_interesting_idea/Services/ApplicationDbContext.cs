using LiteDB;
using UrlShortener.WebApi.Hashi.Model;
using UrlShortener.WebApi.Hashi.Services.Interface;

namespace UrlShortener.WebApi.Hashi.Services
{
    public class ApplicationDbContext : IApplicationDbContext
    {
        private readonly ILiteDatabase _context;

        public ApplicationDbContext(ILiteDatabase context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds URL into database.
        /// </summary>
        /// <param name="urlData">URL</param>
        /// <returns>Row database id.</returns>
        public int AddUrl(ShortenedUrl urlData)
        {
            var db = _context.GetCollection<ShortenedUrl>(BsonAutoId.Int32);
            var id = db.Insert(urlData);

            return id.AsInt32;
        }

        /// <summary>
        /// Gets URL from the database by Row Id.
        /// </summary>
        /// <param name="id">Int32 row id in the database. That is the LIMIT of this application due to such a short number of possible records.</param>
        /// <returns>URL</returns>
        public ShortenedUrl GetUrl(int id)
        {
            var db = _context.GetCollection<ShortenedUrl>();
            var entry = db.Query()
                .Where(x => x.Id.Equals(id))
                .ToList().FirstOrDefault();

            return entry;
        }

        /// <summary>
        /// Checks for existence in the database by simple search query in DB.
        /// </summary>
        /// <param name="longUrl">URL</param>
        /// <returns>Yes or No.</returns>
        public bool CheckIfUrlExists(string longUrl)
        {
            bool exists = false;
            var db = _context.GetCollection<ShortenedUrl>();
            exists = db.Query()
                .Where(x => x.Url.Equals(longUrl))
                .Exists();

            return exists;
        }
    }
}
