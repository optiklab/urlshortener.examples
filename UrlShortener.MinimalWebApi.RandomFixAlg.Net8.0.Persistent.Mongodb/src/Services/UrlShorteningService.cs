﻿using UrlShortener.WebApi.Models;
using UrlShortener.WebApi.Repositories;
using UrlShortener.WebApi.Services.Interfaces;

namespace UrlShortener.WebApi.Services
{
    public class UrlShorteningService : IUrlShorteningService
    {
        public const int NumberOfCharsInShortLink = 7;

        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private readonly Random _random = new();
        private readonly IShortenedUrlsRepository _repository;

        public UrlShorteningService(IShortenedUrlsRepository repository)
        {
            _repository = repository;
        }

        // TODO We might want better generation algorithm:
        // - Pregenerate random values in advance
        // - Use some other algorithm
        public async Task<string> GenerateUniqueCodeAsync()
        {
            var codeChars = new char[NumberOfCharsInShortLink];

            while (true)
            {
                // Variant 1
                // var random = new Random();
                // var code = new string(Enumerable.Repeat(codeChars, NumberOfCharsInShortLink).Select(x => x[random.Next(x.Length)]).ToArray());

                // Variant 2
                //    string urlsafe = string.Empty;
                //    Enumerable.Range(48, 75)
                //      .Where(i => i < 58 || i > 64 && i < 91 || i > 96)
                //      .OrderBy(o => new Random().Next())
                //      .ToList()
                //      .ForEach(i => urlsafe += Convert.ToChar(i)); // Store each char into urlsafe
                //    var token = urlsafe.Substring(new Random().Next(0, urlsafe.Length), new Random().Next(2, 6));

                // Variant 3 - Simplest / easiest
                for (var i = 0; i < NumberOfCharsInShortLink; i++)
                {
                    var randomIndex = _random.Next(Alphabet.Length - 1);

                    codeChars[i] = Alphabet[randomIndex];
                }

                var code = new string(codeChars);

                if (!await _repository.AnyAsync(code))
                {
                    return code;
                }
            }
        }
    }
}
