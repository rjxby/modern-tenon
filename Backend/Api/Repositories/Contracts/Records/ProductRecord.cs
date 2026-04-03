namespace ModernTenon.Api.Repositories.Contracts.Records;

public class ProductRecord
{
    public const int MinNameLength = 3;
    public const int MaxNameLength = 200;

    private Guid _id;
    private string _name = null!;
    private ulong? _priceInCents;

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

    public ulong? PriceInCents
    {
        get => _priceInCents;
        set => _priceInCents = ValidatePriceInCents(value);
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

    private static ulong? ValidatePriceInCents(ulong? priceInCents)
    {
        if (!priceInCents.HasValue)
        {
            return null;
        }

        if (priceInCents.Value == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(priceInCents), "Product price in cents must be greater than zero.");
        }

        return priceInCents.Value;
    }

    private static Guid ValidateId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(id));
        }

        return id;
    }
}
