using System.ComponentModel.DataAnnotations;

namespace UrlShortener.WebApi.Model
{
    public class ShortenedUrl
    {
        public Guid Id { get; set; }

        [Required]
        public string LongUrl { get; set; } = string.Empty;
        [Required]
        public string ShortUrl { get; set; } = string.Empty;
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public DateTime? CreatedOnUtc { get; set; }
    }
}
