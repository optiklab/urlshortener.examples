using Microsoft.EntityFrameworkCore;
using UrlShortener.WebApi;
using UrlShortener.WebApi.DTO;
using UrlShortener.WebApi.Services.Interfaces;
using UrlShortener.WebApi.Services;
using UrlShortener.WebApi.Models;
using UrlShortener.WebApi.Extentions;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddScoped<IUrlShorteningService, UrlShorteningService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

app.MapPost("api/shorten", async (
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
            ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{code}",
            CreatedOnUtc = DateTime.UtcNow
        };

        applicationDbContext.ShortenedUrls.Add(shortenedUrl);

        await applicationDbContext.SaveChangesAsync();

        return Results.Ok(shortenedUrl.ShortUrl);
    }
);

// Alternative: to catch just everything...
// app.MapFallback(async (string code, ApplicationDbContext dbContext) => ...

app.MapGet("api/{code}", async (string code, ApplicationDbContext dbContext) =>
{
    // TODO AY Not very performant. We must use Cache (or better, Distributed Cache here).

    var shortenedUrl = 
        await dbContext.ShortenedUrls.FirstOrDefaultAsync(x => x.Code == code);

    if (shortenedUrl is null)
    {
        return Results.NotFound();
    }

    return Results.Redirect(shortenedUrl.LongUrl);
});

app.MapGet("/get", async (ApplicationDbContext dbContext, HttpContext httpContext) =>
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
