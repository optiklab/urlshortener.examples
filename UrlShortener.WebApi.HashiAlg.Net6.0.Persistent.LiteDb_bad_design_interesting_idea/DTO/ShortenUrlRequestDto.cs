using System.ComponentModel.DataAnnotations;

namespace UrlShortener.WebApi.Hashi.DTO
{
    public class ShortenUrlRequestDto
    {
        [Required]
        public string Url { get; set; }
    }
}