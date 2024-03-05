using LiteDB;
using UrlShortener.WebApi.Model;
using UrlShortener.WebApi.Services.Interface;

namespace UrlShortener.WebApi.Services
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
        public Guid AddUrl(ShortenedUrl urlData)
        {
            var db = _context.GetCollection<ShortenedUrl>(BsonAutoId.Guid);
            var id = db.Insert(urlData);

            return id.AsGuid;
        }

        /// <summary>
        /// Gets URL from the database by Row Id.
        /// </summary>
        /// <param name="code">Code of previousl shortened URL.</param>
        /// <returns>URL</returns>
        public ShortenedUrl GetUrl(string code)
        {
            var db = _context.GetCollection<ShortenedUrl>();
            var entry = db.Query()
                .Where(x => x.Code.Equals(code))
                .ToList().FirstOrDefault();

            return entry;
        }

        /// <summary>
        /// Checks for existence of the code in the database by simple search query in DB.
        /// </summary>
        /// <param name="code">code</param>
        /// <returns>Yes or No.</returns>
        public bool CheckIfCodeExists(string code)
        {
            bool exists = false;
            var db = _context.GetCollection<ShortenedUrl>();
            exists = db.Query()
                .Where(x => x.Code.Equals(code))
                .Exists();

            return exists;
        }

        /// <summary>
        /// Retrieves all existing shortenings.
        /// </summary>
        /// <returns></returns>
        public List<ShortenedUrl> GetAll()
        {
            return _context.GetCollection<ShortenedUrl>().Query().ToList();
        }
    }
}
