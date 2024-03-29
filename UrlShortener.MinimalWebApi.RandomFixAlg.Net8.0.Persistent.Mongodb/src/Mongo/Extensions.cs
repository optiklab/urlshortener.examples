using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace UrlShortener.WebApi.Mongo
{
    public static class Extensions
    {
        public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoOptions>(configuration.GetSection("mongo"));
            services.AddSingleton<MongoClient>(c =>
            {
                var options = c.GetService<IOptions<MongoOptions>>();

                return new MongoClient(options.Value.ConnectionString);
            });

            services.AddSingleton<IMongoDatabase>(c =>
            {
                var options = c.GetService<IOptions<MongoOptions>>();
                var client = c.GetService<MongoClient>();

                return client.GetDatabase(options.Value.Database);
            });

            services.AddSingleton<IDatabaseInitializer, MongoInitializer>();
            services.AddSingleton<IDatabaseSeeder, MongoSeeder>();
        }
    }
}