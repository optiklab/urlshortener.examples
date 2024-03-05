using UrlShortener.WebApi.Services.Interface;
using UrlShortener.WebApi.Services.Interfaces;

namespace UrlShortener.WebApi.Services
{
    public class UrlShorteningService : IUrlShorteningService
    {
        public const int NumberOfCharsInShortLink = 7;

        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private readonly Random _random = new();
        private readonly IApplicationDbContext _dbContext;

        public UrlShorteningService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // TODO We might want better generation algorithm:
        // - Pregenerate random values in advance
        // - Use some other algorithm
        //public async Task<string GenerateUniqueCodeAsync()
        public string GenerateUniqueCode()
        {
            var codeChars = new char[NumberOfCharsInShortLink];

            while (true)
            {
				// var random = new Random();
				// var code = new string(Enumerable.Repeat(codeChars, NumberOfCharsInShortLink)
				//	.Select(x => x[random.Next(x.Length)]).ToArray());
				
                for (var i = 0; i < NumberOfCharsInShortLink; i++)
                {
                    var randomIndex = _random.Next(Alphabet.Length - 1);

                    codeChars[i] = Alphabet[randomIndex];
                }

                var code = new string(codeChars);

                if (!_dbContext.CheckIfCodeExists(code)) // check for duplicates
                {
                    return code;
                }
            }
        }
    }
}
