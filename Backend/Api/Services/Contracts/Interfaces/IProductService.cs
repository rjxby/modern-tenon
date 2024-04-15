using ModernTenon.Api.Services.Contracts.Entities;

namespace ModernTenon.Api.Services.Contracts;

public interface IProductService
{
    Task<PaginationResultEntity<ProductEntity>> GetListAsync(PaginationEntity pagination);

    Task<ProductEntity> GetAsync(Guid id);

    Task<ProductEntity> CreateAsync(ProductEntity product);

    Task<ProductEntity> UpdateAsync(Guid id, ProductEntity product);
}
