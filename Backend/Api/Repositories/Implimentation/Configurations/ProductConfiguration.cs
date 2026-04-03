using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModernTenon.Api.Repositories.Contracts.Records;

namespace ModernTenon.Api.Repositories.Implimentation.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<ProductRecord>
{
    public void Configure(EntityTypeBuilder<ProductRecord> builder)
    {
        builder.HasKey(x => x.Id);

        builder.ToTable(tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_Products_Name_MinLength", "length(Name) >= 3");
            tableBuilder.HasCheckConstraint("CK_Products_PriceInCents_Positive", "\"PriceInCents\" IS NULL OR \"PriceInCents\" >= 1");
        });

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ProductRecord.MaxNameLength);

        builder.Property(x => x.PriceInCents);
    }
}
