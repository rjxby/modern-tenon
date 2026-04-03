using Microsoft.EntityFrameworkCore;

using ModernTenon.Api.Repositories.Implimentation;

namespace ModernTenon.Api.Host;

public static class ApplyDatabaseMigrations
{
    public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        await databaseContext.Database.MigrateAsync();
    }
}
