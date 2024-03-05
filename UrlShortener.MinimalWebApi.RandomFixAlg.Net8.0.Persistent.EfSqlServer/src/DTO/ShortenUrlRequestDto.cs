using Newtonsoft.Json;

namespace UrlShortener.WebApi.DTO
{
    public class ShortenUrlRequestDto
    {
	    [JsonRequired]
	    [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;
    }
}
