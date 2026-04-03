using ModernTenon.Api.Repositories.Contracts;
using ModernTenon.Api.Repositories.Contracts.Records;
using ModernTenon.Api.Services.Contracts;
using ModernTenon.Api.Services.Contracts.Entities;

namespace ModernTenon.Api.Services.Implimentation.Services;

public class ProductService(IProductsRepository productsRepository) : IProductService
{
    private readonly IProductsRepository _productsRepository = productsRepository;

    public async Task<PaginationResultEntity<ProductEntity>> GetListAsync(PaginationEntity pagination)
    {
        var (size, foundRecords) = await _productsRepository.GetListAsync(pagination.Page, pagination.Limit);

        var resultRecords = foundRecords.Select(ProductsFactory.ToEntity);

        var result = new PaginationResultEntity<ProductEntity>(pagination.Page, pagination.Limit, size, resultRecords);
        return result;
    }

    public async Task<ProductEntity> GetAsync(Guid id)
    {
        var record = await GetByIdAsync(id);
        return ProductsFactory.ToEntity(record);
    }

    public async Task<ProductEntity> CreateAsync(ProductEntity product)
    {
        var recordToCreate = ProductsFactory.ToRecord(product);
        var createdRecord = await _productsRepository.CreateAsync(recordToCreate);
        return ProductsFactory.ToEntity(createdRecord);
    }

    public async Task<ProductEntity> UpdateAsync(Guid id, ProductEntity product)
    {
        var recordToUpdate = await GetByIdAsync(id);

        recordToUpdate.Name = product.Name;
        recordToUpdate.PriceInCents = ProductsFactory.ToPriceInCents(product.Price);

        var updatedRecord = await _productsRepository.UpdateAsync(recordToUpdate);
        return ProductsFactory.ToEntity(updatedRecord);
    }

    private async Task<ProductRecord> GetByIdAsync(Guid id)
    {
        var record = await _productsRepository.GetAsync(id);
        if (record == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }

        return record;
    }
}
