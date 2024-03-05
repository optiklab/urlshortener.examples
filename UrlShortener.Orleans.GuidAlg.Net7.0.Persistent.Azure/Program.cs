using Azure.Identity;
using Microsoft.AspNetCore.Http.Extensions;
using UrlShortener.Orleans.WebApp;

var builder = WebApplication.CreateBuilder(args);

// Setup the Silo. Silos are hosts to one or more Grains configured (described in classes).
builder.Host.UseOrleans(siloBuilder =>
{
    // Cluster contains groups of silos to maintain scalability and fault tolerance.
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddAzureBlobGrainStorage("urls",
            // Connect to Blob Storage using DefaultAzureCredentials
            options =>
            {
                options.ConfigureBlobServiceClient(
                    new Uri("https://account.blob.core.windows.net"),
                    new DefaultAzureCredential());
            });
    // Connect to Blob Storage using Connection strings
    // options => options.ConfigureBlobServiceClient(connectionString));
});

var app = builder.Build();

// Use Orleans default grain factory to manage the creation and
// retrieval of grains, by their identifiers.
var grainFactory = app.Services.GetRequiredService<IGrainFactory>();

app.MapGet("/", () => "Hello 2024!");

app.MapGet("/v1/shorten/{*path}",
    async (IGrainFactory grains, HttpRequest request, string path) =>
    {
        var shortenedRouteSegment = Guid.NewGuid().GetHashCode().ToString("X");
        var shortenerGrain = grains.GetGrain<IUrlShortenerGrain>(shortenedRouteSegment);

        await shortenerGrain.SetUrl(shortenedRouteSegment, path);

        var resultBuilder = new UriBuilder(request.GetEncodedUrl())
        {
            Path = $"/go/{shortenedRouteSegment}"
        };

        return Results.Ok(resultBuilder.Uri);
    });

app.MapGet("/v1/go/{shortenedRouteSegment}",
    async (IGrainFactory grains, string shortenedRouteSegment) =>
    {
        var shortenerGrain = grains.GetGrain<IUrlShortenerGrain>(shortenedRouteSegment);

        var url = await shortenerGrain.GetUrl();

        return Results.Redirect(url);
    });

app.Run();
