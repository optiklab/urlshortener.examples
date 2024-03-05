using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UrlShortener.WebApi.DTO;
using UrlShortener.WebApi.Model;
using UrlShortener.WebApi.Services;
using UrlShortener.WebApi.Services.Interface;
using UrlShortener.WebApi.Services.Interfaces;

namespace UrlShortener.WebApi.Controllers
{
    [ApiController]
    public class ShortenerController : ControllerBase
    {
        private readonly IUrlShorteningService _urlShorteningService;
        private readonly ILogger<UrlShorteningService> _logger;
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ShortenerController(IUrlShorteningService urlShorteningService, IApplicationDbContext dbContext, IMapper mapper, ILogger<UrlShorteningService> logger)
        {
            _urlShorteningService = urlShorteningService;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("api/shorten")]
        public IActionResult ShortenUrl([FromBody] ShortenUrlRequestDto requestUrlDataDto)
        {
            if (requestUrlDataDto == null)
                return BadRequest();

            if (!Uri.TryCreate(requestUrlDataDto.Url, UriKind.Absolute, out Uri result))
                ModelState.AddModelError("URL", "The specified URL is invalid.");

            _logger.LogInformation("URL to shorten: " + requestUrlDataDto.Url);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //var code = await _urlShorteningService.GenerateUniqueCodeAsync();
            var code = _urlShorteningService.GenerateUniqueCode();

            var shortUrl = $"{this.Request.Scheme}://{this.Request.Host}/api/{code}";

            var shortenedUrl = new ShortenedUrl
            {
                LongUrl = requestUrlDataDto.Url,
                Code = code,
                ShortUrl = shortUrl,
                CreatedOnUtc = DateTime.UtcNow
            };

            _dbContext.AddUrl(shortenedUrl);

            return Created("shortUrl", shortUrl);
        }

        [HttpGet("api/{code}")]
        public IActionResult GetUrl(string code)
        {
            if (String.IsNullOrEmpty(code))
                return BadRequest();

            _logger.LogInformation("URL to redirect: " + code);

            // TODO Not very performant. We must use Cache (or better, Distributed Cache here).

            var shortenedUrl = _dbContext.GetUrl(code);

            if (shortenedUrl is null)
            {
                return NotFound();
            }

            return RedirectPermanent(shortenedUrl.LongUrl);
        }

        [HttpGet("get")]
        public IActionResult GetAll()
        {
            var allShortUrlStrings = _dbContext.GetAll().Select(x => _mapper.Map<ShortenUrlResponseDto>(x)).ToList();

            return Ok($"Here is the list of urls. List: {JsonConvert.SerializeObject(allShortUrlStrings)}");
        }

        [HttpGet("/delete/{code}")]
        public IActionResult Delete(string code)
        {
            if (code is null)
            {
                return NotFound();
            }

            // TODO

            return Ok("Deleted!");
        }
    }
}
