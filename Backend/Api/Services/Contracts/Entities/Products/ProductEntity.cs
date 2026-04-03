using System.Diagnostics.CodeAnalysis;

namespace ModernTenon.Api.Services.Contracts.Entities;

public class ProductEntity
{
    public const int MinNameLength = 3;
    public const int MaxNameLength = 200;

    private Guid _id;
    private string _name = null!;
    private decimal? _price;

    [SetsRequiredMembers]
    public ProductEntity(Guid id, string name)
    {
        Id = ValidateId(id);
        Name = name;
    }

    [SetsRequiredMembers]
    public ProductEntity(Guid id, string name, decimal price)
    {
        Id = ValidateId(id);
        Name = name;
        Price = price;
    }

    public required Guid Id
    {
        get => _id;
        set => _id = ValidateId(value);
    }

    public required string Name
    {
        get => _name;
        set => _name = ValidateName(value);
    }

    public decimal? Price
    {
        get => _price;
        set => _price = ValidatePrice(value);
    }

    private static Guid ValidateId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(id));
        }

        return id;
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name is required.", nameof(name));
        }

        var normalizedName = name.Trim();
        if (normalizedName.Length < MinNameLength)
        {
            throw new ArgumentException($"Product name must be at least {MinNameLength} characters long.", nameof(name));
        }

        if (normalizedName.Length > MaxNameLength)
        {
            throw new ArgumentException($"Product name cannot exceed {MaxNameLength} characters.", nameof(name));
        }

        return normalizedName;
    }

    private static decimal? ValidatePrice(decimal? price)
    {
        if (!price.HasValue)
        {
            return null;
        }

        if (price.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Product price must be greater than zero.");
        }

        if (decimal.Round(price.Value, 2) != price.Value)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Product price cannot have more than 2 decimal places.");
        }

        return price.Value;
    }
}
