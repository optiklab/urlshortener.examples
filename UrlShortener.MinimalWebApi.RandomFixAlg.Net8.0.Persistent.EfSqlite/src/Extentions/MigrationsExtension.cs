using Microsoft.EntityFrameworkCore;

namespace UrlShortener.WebApi.Extentions
{
    public static class MigrationsExtension
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Database.Migrate();
        }
    }
}
