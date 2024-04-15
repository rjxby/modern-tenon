using ModernTenon.Api.Repositories.Contracts.Records;

namespace ModernTenon.Api.Repositories.Contracts;

public interface IProductsRepository
{
    Task<ProductRecord?> GetAsync(Guid id);

    Task<ProductRecord> CreateAsync(ProductRecord record);

    Task<(int size, IEnumerable<ProductRecord> results)> GetListAsync(int page, int limit);

    Task<ProductRecord> UpdateAsync(ProductRecord record);
}
