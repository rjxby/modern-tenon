namespace ModernTenon.Api.Repositories.Contracts.Records;

public class ProductRecord
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public ulong? PriceInCents { get; set; }
}
