using UrlShortener.WebApi.Services.Interface;
using LiteDB;
using UrlShortener.WebApi.Services;
using Microsoft.OpenApi.Models;
using log4net;
using log4net.Config;
using System.Reflection;
using UrlShortener.WebApi.Services.Interfaces;
using UrlShortener.WebApi.Profiles;

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

            app.UseMvc(); // ? from JoyEnergy

			app.UseHttpsRedirection();

			app.UseAuthorization();

            // app.MapControllers();

			// app.Run(); // From Hashi Min Web Api
        }
    }
}
