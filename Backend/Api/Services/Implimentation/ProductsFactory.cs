using ModernTenon.Api.Repositories.Contracts.Records;
using ModernTenon.Api.Services.Contracts.Entities;

namespace ModernTenon.Api.Services.Implimentation;

internal static class ProductsFactory
{
    private const decimal Cents = 100m;

    public static ProductEntity ToEntity(ProductRecord record)
    {
        return record.PriceInCents.HasValue
            ? new ProductEntity(record.Id, record.Name, record.PriceInCents.Value / Cents)
            : new ProductEntity(record.Id, record.Name);
    }

    public static ProductRecord ToRecord(ProductEntity entity)
    {
        return new ProductRecord
        {
            Id = entity.Id,
            Name = entity.Name,
            PriceInCents = ToPriceInCents(entity.Price)
        };
    }
    
    public static ulong? ToPriceInCents(decimal? price)
    {
        if (!price.HasValue)
        {
            return null;
        }

        if (decimal.Round(price.Value, 2) != price.Value)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Product price cannot have more than 2 decimal places.");
        }

        return price.HasValue
            ? checked((ulong)decimal.Truncate(price.Value * Cents))
            : null;
    }

}
