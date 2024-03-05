namespace UrlShortener.WebApi.Mongo
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync();
    }
}