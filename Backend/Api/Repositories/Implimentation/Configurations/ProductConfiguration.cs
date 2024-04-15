using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModernTenon.Api.Repositories.Contracts.Records;

namespace ModernTenon.Api.Repositories.Implimentation.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<ProductRecord>
{
    public void Configure(EntityTypeBuilder<ProductRecord> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name);
        builder.Property(x => x.PriceInCents);
    }
}
