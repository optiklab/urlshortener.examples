﻿using Asp.Versioning;
using LiteDB;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using UrlShortener.WebApi.OpenApi;
using UrlShortener.WebApi.Profiles;
using UrlShortener.WebApi.Services;
using UrlShortener.WebApi.Services.Interface;
using UrlShortener.WebApi.Services.Interfaces;

namespace UrlShortener.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false);
			
			services.AddControllers();

			// Add LiteDB
			services.AddSingleton<ILiteDatabase, LiteDatabase>(_ => new LiteDatabase("shortner-service.db"));
			services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
			services.AddScoped<IUrlShorteningService, UrlShorteningService>();

            //services.AddSingleton((IServiceProvider arg) => initial_data); // Possible

            services.AddAutoMapper(typeof(UrlProfile));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());

            // Versioning guidelines: https://www.milanjovanovic.tech/blog/api-versioning-in-aspnetcore
            services.AddApiVersioning(options =>
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
            })
            .EnableApiVersionBinding();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
			// Configure service log
			var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
			XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
			ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
			log.Info("Log config file loaded");

            // app.ConfigureExceptionHandler(log);

            app.UseMvc();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(app =>
            {
                app.MapControllers();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();

                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));
            }
        }
    }
}
