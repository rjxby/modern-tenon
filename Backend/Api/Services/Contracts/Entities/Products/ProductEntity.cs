using System.Diagnostics.CodeAnalysis;

namespace ModernTenon.Api.Services.Contracts.Entities;

public class ProductEntity
{
    [SetsRequiredMembers]
    public ProductEntity(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    [SetsRequiredMembers]
    public ProductEntity(Guid id, string name, double price)
    {
        Id = id;
        Name = name;
        Price = price;
    }

    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public double? Price { get; set; }
}
