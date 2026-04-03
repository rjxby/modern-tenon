using Microsoft.EntityFrameworkCore;

using ModernTenon.Api.Repositories.Contracts;
using ModernTenon.Api.Repositories.Implimentation;

namespace ModernTenon.Api.Host;

public static class SetupRepositoryLayer
{
    public static IServiceCollection AddRepositoryLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        services.AddDbContextPool<DatabaseContext>(o =>
        {
            o.UseSqlite(connectionString);
        });

        services.AddTransient<IProductsRepository, ProductsRepository>();

        return services;
    }
}
