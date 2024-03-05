namespace UrlShortener.WebApi.Services.Interfaces
{
    public interface IUrlShorteningService
    {
        //Task<string> GenerateUniqueCodeAsync();
        string GenerateUniqueCode();
    }
}
