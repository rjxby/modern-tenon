using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ModernTenon.Api.IntegrationTests;

public abstract class TestFixture<TEntryPoint, TContext> : IAsyncLifetime
    where TEntryPoint : class
    where TContext : DbContext
{
    private readonly WebApplicationFactory<TEntryPoint> _factory;

    public HttpClient HttpClient => _factory.CreateClient();

    public TContext DbContext => _factory.Services.GetRequiredService<TContext>();

    public async Task InitializeAsync()
    {
        await DbContext.Database.OpenConnectionAsync();
        await ApplyMigrationsAsync(DbContext);
        await SeedDatabaseAsync(DbContext);
    }

    public async Task DisposeAsync()
    {
        await DbContext.Database.CloseConnectionAsync();
        await _factory.DisposeAsync();
    }

    protected TestFixture()
    {
        _factory = new WebApplicationFactory<TEntryPoint>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.ConfigureServices(services => ReplaceDbContext(services));
            });
    }

    protected abstract Task SeedDatabaseAsync(TContext context);

    private async Task ApplyMigrationsAsync(TContext context)
    {
        await context.Database.MigrateAsync();
    }

    private void ReplaceDbContext(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TContext>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContextPool<TContext>(options =>
        {
            options.UseSqlite($"DataSource={Guid.NewGuid()};Mode=Memory;cache=shared");
        });
    }
}