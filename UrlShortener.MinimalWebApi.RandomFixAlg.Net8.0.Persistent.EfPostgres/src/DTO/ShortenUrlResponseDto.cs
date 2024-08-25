using Newtonsoft.Json;

namespace UrlShortener.WebApi.DTO
{
    public class ShortenUrlResponseDto
    {
        [JsonRequired]
        [JsonProperty("long-url")]
        public string LongUrl { get; set; } = string.Empty;

        [JsonRequired]
        [JsonProperty("short-url")]
        public string ShortUrl { get; set; } = string.Empty;

        [JsonRequired]
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonRequired]
        [JsonProperty("createddate")]
        public DateTime? CreatedOnUtc { get; set; }
    }
}
