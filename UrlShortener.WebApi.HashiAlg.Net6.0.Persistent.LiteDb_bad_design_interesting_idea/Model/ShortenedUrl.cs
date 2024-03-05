using System.ComponentModel.DataAnnotations;

namespace UrlShortener.WebApi.Hashi.Model
{
    public class ShortenedUrl
    {
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public DateTime ShorteningDateTime { get; set; }
    }
}
