using AutoMapper;
using UrlShortener.WebApi.DTO;
using UrlShortener.WebApi.Models;

namespace UrlShortener.WebApi.Profiles
{
    public class UrlProfile : Profile
    {
        public UrlProfile()
        {
            //CreateMap<ShortenedUrl, ShortenUrlResponseDto>().ReverseMap();
            //CreateMap<List<ShortenedUrl>, List<ShortenUrlResponseDto>>().ReverseMap();
        }
    }
}