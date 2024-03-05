using UrlShortener.WebApi.DTO;
using UrlShortener.WebApi.Services.Interfaces;
using UrlShortener.WebApi.Services;
using UrlShortener.WebApi.Models;
using UrlShortener.WebApi.Mongo;
using AutoMapper;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using UrlShortener.WebApi.Repositories;
using UrlShortener.WebApi.Profiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMongoDb(builder.Configuration);

builder.Services.AddScoped<IUrlShorteningService, UrlShorteningService>();
builder.Services.AddScoped<IShortenedUrlsRepository, ShortenedUrlsRepository>();

builder.Services.AddAutoMapper(typeof(UrlProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// This is required because of Scopes Validation
// https://stackoverflow.com/questions/48590579/cannot-resolve-scoped-service-from-root-provider-net-core-2
// and
// https://stackoverflow.com/questions/50198609/asp-net-core-2-serviceprovideroptions-validatescopes-property/50198738#50198738
var scopedFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopedFactory.CreateScope())
{
    var initializer = app.Services.GetService<IDatabaseInitializer>();

    if (initializer != null)
        await initializer.InitializeAsync();
}

app.MapPost("api/shorten", async (
    ShortenUrlRequestDto request,
    IUrlShorteningService urlShorteningService,
    IShortenedUrlsRepository shortenedUrlsRepository,
    HttpContext httpContext) =>
    {
        if (request == null)
            return Results.BadRequest("The specified URL shouldn't be empty.");

        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out Uri result))
            return Results.BadRequest("The specified URL is invalid.");

        var code = await urlShorteningService.GenerateUniqueCodeAsync();

        var shortenedUrl = new ShortenedUrl
        {
            Id = Guid.NewGuid(),
            LongUrl = request.Url,
            Code = code,
            ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{code}",
            CreatedOnUtc = DateTime.UtcNow
        };

        await shortenedUrlsRepository.AddAsync(shortenedUrl);

        return Results.Ok(shortenedUrl.ShortUrl);
    }
);

// Alternative: to catch just everything...
// app.MapFallback(async (string code, ApplicationDbContext dbContext) => ...

app.MapGet("api/{code}", async (string code, IShortenedUrlsRepository shortenedUrlsRepository) =>
{
    // TODO Not very performant. We must use Cache (or better, Distributed Cache here).

    var shortenedUrl =  await shortenedUrlsRepository.GetAsync(code);

    if (shortenedUrl is null)
    {
        return Results.NotFound();
    }

    return Results.Redirect(shortenedUrl.LongUrl);
});

app.MapGet("/get", async (IShortenedUrlsRepository shortenedUrlsRepository, HttpContext httpContext, IMapper mapper) =>
{
    var allShortUrlStrings = (await shortenedUrlsRepository.BrowseAsync())
    .Select(x =>
        new ShortenUrlResponseDto
        {
            Code = x.Code,
            LongUrl = x.LongUrl,

            // TODO I'm passing IMapper, but not really using it here, because of this custom modification.
            ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{x.Code}",
            CreatedOnUtc = x.CreatedOnUtc
        })
    .ToList();

    return $"Here is the list of urls. List: {JsonConvert.SerializeObject(allShortUrlStrings)}";
});

app.MapDelete("/delete/{code}", (string code) =>
{
    if (code is null)
    {
        return Results.NotFound();
    }
	
	// TODO

    return Results.Ok("Deleted!");
});

app.UseHttpsRedirection();

app.Run();
