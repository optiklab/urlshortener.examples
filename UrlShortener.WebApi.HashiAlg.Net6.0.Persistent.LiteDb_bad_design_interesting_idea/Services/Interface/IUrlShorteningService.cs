using UrlShortener.WebApi.Hashi.DTO;

namespace UrlShortener.WebApi.Hashi.Services.Interface
{
    public interface IUrlShorteningService
    {
        ShortenUrlRequestDto AddUrlData(ShortenUrlRequestDto urlDataDto, string requestScheme, string requestHost);

        ShortenUrlRequestDto GetUrlData(string shortUrl);

        bool CheckUserAgent(string headersUserAgent);

        bool CheckIfUrlExists(ShortenUrlRequestDto urlDataDto);
    }
}
