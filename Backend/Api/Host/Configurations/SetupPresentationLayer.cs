namespace ModernTenon.Api.Host;

public static class SetupPresentationLayer
{
    public static IServiceCollection AddPresentationLayer(this IServiceCollection services)
    {        
        services.AddSwagger();

        services.AddExceptionHandler<ExceptionHandler>();

        services.AddAutoMapper(typeof(MappingProfile));

        return services;
    }
}
