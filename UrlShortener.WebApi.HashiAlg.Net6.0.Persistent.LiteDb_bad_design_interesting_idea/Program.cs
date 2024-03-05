using LiteDB;
using log4net.Config;
using log4net;
using Microsoft.OpenApi.Models;
using System.Reflection;
using UrlShortener.WebApi.Hashi.Services;
using UrlShortener.WebApi.Hashi.Services.Interface;
using UrlShortener.WebApi.Hashi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add LiteDB
builder.Services.AddSingleton<ILiteDatabase, LiteDatabase>(_ => new LiteDatabase("shortner-service.db"));
builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
builder.Services.AddScoped<IUrlHelper, UrlHelper>();
builder.Services.AddScoped<IUrlShorteningService, UrlShorteningService>();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UrlShortener.WebApi.Hashi", Version = "v1" });
});

var app = builder.Build();

// Configure service log
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
log.Info("Log config file loaded");

app.ConfigureExceptionHandler(log);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UrlShortener.WebApi.Hashi v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
