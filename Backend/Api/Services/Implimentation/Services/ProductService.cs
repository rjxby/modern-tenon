using AutoMapper;

using ModernTenon.Api.Repositories.Contracts;
using ModernTenon.Api.Repositories.Contracts.Records;
using ModernTenon.Api.Services.Contracts;
using ModernTenon.Api.Services.Contracts.Entities;

namespace ModernTenon.Api.Services.Implimentation.Services;

public class ProductService(IMapper mapper, IProductsRepository productsRepository) : IProductService
{
    private readonly IMapper _mapper = mapper;
    private readonly IProductsRepository _productsRepository = productsRepository;

    public async Task<PaginationResultEntity<ProductEntity>> GetListAsync(PaginationEntity pagination)
    {
        var (size, foundRecords) = await _productsRepository.GetListAsync(pagination.Page, pagination.Limit);

        var resultRecords = _mapper.Map<IEnumerable<ProductEntity>>(foundRecords);

        var result = new PaginationResultEntity<ProductEntity>(pagination.Page, pagination.Limit, size, resultRecords);
        return result;
    }

    public async Task<ProductEntity> GetAsync(Guid id)
    {
        var record = await GetByIdAsync(id);
        return _mapper.Map<ProductEntity>(record);
    }

    public async Task<ProductEntity> CreateAsync(ProductEntity product)
    {
        var recordToCreate = _mapper.Map<ProductRecord>(product);
        var createdRecord = await _productsRepository.CreateAsync(recordToCreate);
        return _mapper.Map<ProductEntity>(createdRecord);
    }

    public async Task<ProductEntity> UpdateAsync(Guid id, ProductEntity product)
    {
        var recordToUpdate = await GetByIdAsync(id);

        _mapper.Map(product, recordToUpdate);

        var updatedRecord = await _productsRepository.UpdateAsync(recordToUpdate);
        return _mapper.Map<ProductEntity>(updatedRecord);
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
