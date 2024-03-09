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
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using UrlShortener.WebApi.OpenApi;
using Asp.Versioning;
using Asp.Versioning.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddScoped<IUrlShorteningService, UrlShorteningService>();
builder.Services.AddScoped<IShortenedUrlsRepository, ShortenedUrlsRepository>();
builder.Services.AddAutoMapper(typeof(UrlProfile));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());

// Versioning guidelines: https://www.milanjovanovic.tech/blog/api-versioning-in-aspnetcore
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true; // the version is generated and displayed in the header section as shown in the image below.
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    //options.ApiVersionReader = ApiVersionReader.Combine(
    //    new UrlSegmentApiVersionReader(),
    //    new HeaderApiVersionReader("X-Api-Version"));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

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

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))
    .ReportApiVersions()
    .Build();

app.MapPost("/api/v{version:apiVersion}/shorten", async (
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
            ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/v{httpContext.GetRequestedApiVersion()}/{code}",
            CreatedOnUtc = DateTime.UtcNow
        };

        await shortenedUrlsRepository.AddAsync(shortenedUrl);

        return Results.Ok(shortenedUrl.ShortUrl);
    }
).WithApiVersionSet(apiVersionSet)
.MapToApiVersion(1, 0);

// Alternative: to catch just everything...
// app.MapFallback(async (string code, ApplicationDbContext dbContext) => ...

app.MapGet("/api/v{version:apiVersion}/{code}", async (string code, IShortenedUrlsRepository shortenedUrlsRepository) =>
{
    // TODO Not very performant. We must use Cache (or better, Distributed Cache here).

    var shortenedUrl =  await shortenedUrlsRepository.GetAsync(code);

    if (shortenedUrl is null)
    {
        return Results.NotFound();
    }

    return Results.Redirect(shortenedUrl.LongUrl);
}).WithApiVersionSet(apiVersionSet)
.MapToApiVersion(1, 0);

app.MapGet("/api/v{version:apiVersion}/get", async (IShortenedUrlsRepository shortenedUrlsRepository, HttpContext httpContext, IMapper mapper) =>
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
}).WithApiVersionSet(apiVersionSet)
.MapToApiVersion(1, 0);

app.MapDelete("/api/v{version:apiVersion}/delete/{code}", (string code) =>
{
    if (code is null)
    {
        return Results.NotFound();
    }
	
	// TODO

    return Results.Ok("Deleted!");
}).WithApiVersionSet(apiVersionSet)
.MapToApiVersion(1, 0)
.MapToApiVersion(2, 0);

// Configure the HTTP request pipeline. It's very important to do AFTER all Mappings (MapGet and MapPost) to make it SEE all versions.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            foreach (var description in app.DescribeApiVersions())
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName);
            }
        });
}

app.UseHttpsRedirection();

app.Run();
