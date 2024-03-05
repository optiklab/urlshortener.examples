namespace UrlShortener.WebApi.Mongo
{
    public interface IDatabaseSeeder
    {
        Task SeedAsync();
    }
}