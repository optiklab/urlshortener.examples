using Microsoft.EntityFrameworkCore;
namespace UrlShortener.WebApi.Extentions
{
    public static class MigrationsExtension
    {
        /// <summary>
        /// While productive for local development and testing of migrations, this approach is INAPPROPRIATE for managing production databases.
        /// Read more https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli
        /// </summary>
        /// <param name="app"></param>
        public static void ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Warning! TODO AY
            // Carefully consider before using this approach in production.
            // Experience has shown that the simplicity of this deployment strategy is outweighed by the issues it creates.
            // Consider generating SQL scripts from migrations instead.

            // Don't call EnsureCreated() before Migrate(). EnsureCreated() bypasses Migrations to create the schema, which causes Migrate() to fail.

            // If it fails because DB already created during your debugging...simply comment this line :)
            // In prod, it's better to use "EF migration scripts like $> dotnet ef database update ...".
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
            }
        }
    }
}
