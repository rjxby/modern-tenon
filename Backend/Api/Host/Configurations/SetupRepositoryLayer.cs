using Microsoft.EntityFrameworkCore;

using ModernTenon.Api.Repositories.Contracts;
using ModernTenon.Api.Repositories.Implimentation;

namespace ModernTenon.Api.Host;

public static class SetupRepositoryLayer
{
    public static IServiceCollection AddRepositoryLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<DatabaseContext>(o =>
        {
            o.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddTransient<IProductsRepository, ProductsRepository>();

        return services;
    }
}
