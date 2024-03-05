using Microsoft.AspNetCore.Http.Extensions;
using UrlShortener.Orleans.WebApp;

var builder = WebApplication.CreateBuilder(args);

// Setup the Silo. Silos are hosts to one or more Grains configured (described in classes).
builder.Host.UseOrleans(siloBuilder =>
{
    // Cluster contains groups of silos to maintain scalability and fault tolerance.
    siloBuilder.UseLocalhostClustering();

    // MemoryStorage is a simple storage provider that does not really use a persistent data store underneath.
    // It is convenient to learn to work with Storage Providers quickly, but is not intended to be used in real scenarios.
    siloBuilder.AddMemoryGrainStorage("urls");
	// Basicly, this is an analog of:
	// public class Database : Dictionary<string, string>{
	// then
	// private readonly Database _database;
    //}
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
            Path = $"/v1/go/{shortenedRouteSegment}"
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
