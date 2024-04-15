using Microsoft.EntityFrameworkCore;

using ModernTenon.Api.Repositories.Contracts.Records;
using ModernTenon.Api.Repositories.Implimentation.Configurations;

namespace ModernTenon.Api.Repositories.Implimentation;

public class DatabaseContext: DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public DbSet<ProductRecord> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);
    }
}
