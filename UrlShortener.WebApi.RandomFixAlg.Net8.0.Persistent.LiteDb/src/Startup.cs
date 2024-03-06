using UrlShortener.WebApi.Services.Interface;
using LiteDB;
using UrlShortener.WebApi.Services;
using Microsoft.OpenApi.Models;
using log4net;
using log4net.Config;
using System.Reflection;
using UrlShortener.WebApi.Services.Interfaces;
using UrlShortener.WebApi.Profiles;
using Asp.Versioning;

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

            // https://www.tatvasoft.com/blog/different-methods-of-api-versioning-routing-in-asp-net-core/
            // Using Microsoft.AspNetCore.Mvc.Versioning package
            //services.AddApiVersioning(config =>    
            //{
            //    config.DefaultApiVersion = new ApiVersion(1, 0); //ApiVersion.Neutral;
            //    config.AssumeDefaultVersionWhenUnspecified = true;
            //    config.ReportApiVersions = true; // the version is generated and displayed in the header section as shown in the image below.
            //    config.ApiVersionReader = new HeaderApiVersionReader("api-version");
            //});

            // https://www.milanjovanovic.tech/blog/api-versioning-in-aspnetcore
            // Using Asp.Versioning.Http and Asp.Versioning.Mvc.ApiExplorer packages
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
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "UrlShortener.WebApi.RandomFixAlg.Net8.0.Persistent.LiteDb", Version = "v1" });
			});
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

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UrlShortener.WebApi.RandomFixAlg.Net8.0.Persistent.LiteDb v1"));
            }

            app.UseMvc();

			app.UseHttpsRedirection();

            app.UseRouting(); // UseRouting and UseEndpoints are the two ways to register Routing

            app.UseAuthorization();

            app.UseEndpoints(app =>
            {
                app.MapControllers();
            });

            // app.Run(); // From Hashi Min Web Api
        }
    }
}
