using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UrlShortener.WebApi.OpenApi
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo()
                    {
                        Title = "UrlShortener.MinimalWebApi.RandomFixAlg.Net8.0.Persistent.EfSqlite",
                        Description = "Url Shortener API made with Minimal WebApi .NET 8.0, persisted in SQLite (via Entity Framework) and Base62 algorithm of generating unique shortened values.",
                        Version = description.ApiVersion.ToString(),
                    });
            }
        }
    }
}
