using System.ComponentModel.DataAnnotations;

namespace UrlShortener.WebApi.DTO
{
    public class ShortenUrlRequestDto
    {
        [Required]
        public string Url { get; set; }
    }
}