using AutoMapper;
using UrlShortener.WebApi.Hashi.DTO;
using UrlShortener.WebApi.Hashi.Model;

namespace UrlShortener.WebApi.Hashi.Profiles
{
    public class UrlProfile : Profile
    {
        public UrlProfile()
        {
            CreateMap<ShortenedUrl, ShortenUrlRequestDto>().ReverseMap();
        }
    }
}