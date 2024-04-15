using ModernTenon.Api.Services.Contracts;
using ModernTenon.Api.Services.Implimentation.Services;

namespace ModernTenon.Api.Host;

public static class SetupServiceLayer
{
    public static IServiceCollection AddServiceLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Services.Implimentation.MappingProfile));

        services.AddTransient<IProductService, ProductService>();

        return services;
    }
}
