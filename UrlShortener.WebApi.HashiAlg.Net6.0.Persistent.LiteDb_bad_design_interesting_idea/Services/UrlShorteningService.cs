using AutoMapper;
using UrlShortener.WebApi.Hashi.DTO;
using UrlShortener.WebApi.Hashi.Model;
using UrlShortener.WebApi.Hashi.Services.Interface;

namespace UrlShortener.WebApi.Hashi.Services
{
    public class UrlShorteningService : IUrlShorteningService
    {
        private IApplicationDbContext _dbContext;
        private IUrlHelper _urlHelper;
        private IMapper _mapper;
        private readonly ILogger<UrlShorteningService> _logger;

        public UrlShorteningService(IApplicationDbContext dbContext, IUrlHelper urlHelper, IMapper mapper, ILogger<UrlShorteningService> logger)
        {
            _dbContext = dbContext;
            _urlHelper = urlHelper;
            _mapper = mapper;
            _logger = logger;
        }

        public ShortenUrlRequestDto GetUrlData(string shortUrl)
        {
            var id = _urlHelper.GetId(shortUrl);
            var urlData = _dbContext.GetUrl(id);
            var urlDataDto = _mapper.Map<ShortenUrlRequestDto>(urlData);

            return urlDataDto;
        }

        public ShortenUrlRequestDto AddUrlData(ShortenUrlRequestDto urlDataDto, string requestScheme, string requestHost)
        {
            var newEntry = new ShortenedUrl
            {
                Url = urlDataDto.Url,
                ShorteningDateTime = DateTime.Now.Date
            };

            var id = _dbContext.AddUrl(newEntry);
            ShortenUrlRequestDto responseUrlDataDto = new ShortenUrlRequestDto() { Url = $"{requestScheme}://{requestHost}/{_urlHelper.GetShortUrl(id)}" };

            _logger.LogInformation("Shortened URL is: " + responseUrlDataDto.Url);

            return responseUrlDataDto;
        }

        public bool CheckUserAgent(string headersUserAgent)
        {
            bool userAgentBrowser = false;
            // Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.114 Safari/537.36
            // Just to avoid version issues
            if (headersUserAgent.Contains("Mozilla") ||
                   headersUserAgent.Contains("AppleWebKit") ||
                   headersUserAgent.Contains("Chrome") ||
                   headersUserAgent.Contains("Safari") ||
                   headersUserAgent.Contains("Edg"))
                userAgentBrowser = true;

            return userAgentBrowser;
        }

        public bool CheckIfUrlExists(ShortenUrlRequestDto urlDataDto)
        {
            return _dbContext.CheckIfUrlExists(urlDataDto.Url); // TODO Not very performant. We must use Cache (or better, Distributed Cache here).
        }
    }
}
