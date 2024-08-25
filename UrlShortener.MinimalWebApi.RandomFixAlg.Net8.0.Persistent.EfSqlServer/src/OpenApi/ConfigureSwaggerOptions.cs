using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UrlShortener.WebApi.OpenApi
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        /// <param name="provider">
        ///   The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.
        /// </param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

        /// <inheritdoc/>
        public void Configure(SwaggerGenOptions options)
        {
            // add a swagger document for each discovered API version
            // note: you might choose to skip or document deprecated API versions differently
            foreach (var description in _provider.ApiVersionDescriptions)
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = $"UrlShortener.MinimalWebApi.RandomFixAlg.Net8.0.Persistent.EfSqlServer {description.ApiVersion.ToString()}",
                Version = description.ApiVersion.ToString(),
                Description = "Url Shortener API made with Minimal WebApi .NET 8.0, persisted in SQLServer (via Entity Framework) and Base62 algorithm of generating unique shortened values.",
                Contact = new OpenApiContact { Name = "Anton Yarkov", Email = "anton.yarkov@gmail.com" },
                License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
            };

            if (description.IsDeprecated)
                info.Description += " This API version has been deprecated.";

            return info;
        }
    }
}
