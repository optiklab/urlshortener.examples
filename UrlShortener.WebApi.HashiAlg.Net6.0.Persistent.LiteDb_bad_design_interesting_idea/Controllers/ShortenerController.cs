using Microsoft.AspNetCore.Mvc;
using UrlShortener.WebApi.Hashi.DTO;
using UrlShortener.WebApi.Hashi.Services;
using UrlShortener.WebApi.Hashi.Services.Interface;

namespace UrlShortener.WebApi.Hashi.Controllers
{
    [ApiController]
    public class ShortenerController : ControllerBase
    {
        private readonly IUrlShorteningService _urlShorteningService;
        private readonly ILogger<UrlShorteningService> _logger;

        public ShortenerController(IUrlShorteningService urlShorteningService, ILogger<UrlShorteningService> logger)
        {
            _urlShorteningService = urlShorteningService;
            _logger = logger;
        }


        [HttpPost("shorten")]
        public IActionResult ShortenUrl([FromBody] ShortenUrlRequestDto requestUrlDataDto)
        {
            if (requestUrlDataDto == null)
                return BadRequest();

            if (!Uri.TryCreate(requestUrlDataDto.Url, UriKind.Absolute, out Uri result))
                ModelState.AddModelError("URL", "URL shouldn't be empty");

            _logger.LogInformation("URL to shorten: " + requestUrlDataDto.Url);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_urlShorteningService.CheckIfUrlExists(requestUrlDataDto))
                ModelState.AddModelError("Url", "Url is allready shortened");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var responseUrlDataDto = _urlShorteningService.AddUrlData(requestUrlDataDto, this.Request.Scheme, this.Request.Host.ToString());

            return Created("shortUrl", responseUrlDataDto);
        }

        [HttpGet("{shortUrl}")]
        public IActionResult GetUrl(string shortUrl)
        {
            if (String.IsNullOrEmpty(shortUrl))
                return BadRequest();

            _logger.LogInformation("URL to redirect: " + shortUrl);

            var urlDataDto = _urlShorteningService.GetUrlData(shortUrl);

            if (urlDataDto == null)
                return NotFound();

            var userAgentBrowser = _urlShorteningService.CheckUserAgent(this.Request.Headers["User-Agent"].ToString());

            if (userAgentBrowser)
                return RedirectPermanent(urlDataDto.Url);
            return Ok(urlDataDto);
        }
    }
}
