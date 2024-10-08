using Microsoft.EntityFrameworkCore;
using UrlShortener.WebApi;
using UrlShortener.WebApi.DTO;
using UrlShortener.WebApi.Services.Interfaces;
using UrlShortener.WebApi.Services;
using UrlShortener.WebApi.Models;
using UrlShortener.WebApi.Extentions;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using UrlShortener.WebApi.OpenApi;
using Asp.Versioning;
using Asp.Versioning.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddScoped<IUrlShorteningService, UrlShorteningService>();

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

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))
    .ReportApiVersions()
    .Build();

app.MapPost("/api/v{version:apiVersion}/shorten", async (
    ShortenUrlRequestDto request,
    IUrlShorteningService urlShorteningService,
    ApplicationDbContext applicationDbContext,
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

        applicationDbContext.ShortenedUrls.Add(shortenedUrl);

        await applicationDbContext.SaveChangesAsync();

        return Results.Ok(shortenedUrl.ShortUrl);
    }
).WithApiVersionSet(apiVersionSet)
.MapToApiVersion(1, 0);

// Alternative: to catch just everything...
// app.MapFallback(async (string code, ApplicationDbContext dbContext) => ...

app.MapGet("/api/v{version:apiVersion}/{code}", async (string code, ApplicationDbContext dbContext) =>
{
    // TODO AY Not very performant. We must use Cache (or better, Distributed Cache here).

    var shortenedUrl = 
        await dbContext.ShortenedUrls.FirstOrDefaultAsync(x => x.Code == code);

    if (shortenedUrl is null)
    {
        return Results.NotFound();
    }

    return Results.Redirect(shortenedUrl.LongUrl);
}).WithApiVersionSet(apiVersionSet)
.MapToApiVersion(1, 0);

app.MapGet("/api/v{version:apiVersion}/get", async (ApplicationDbContext dbContext, HttpContext httpContext) =>
{
    var allShortUrlStrings = await dbContext.ShortenedUrls.Select(x => 
		new ShortenUrlResponseDto
		{
			Code = x.Code,
			LongUrl = x.LongUrl,
			ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{x.Code}",
			CreatedOnUtc = x.CreatedOnUtc
		}).ToListAsync();

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

    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.Run();
