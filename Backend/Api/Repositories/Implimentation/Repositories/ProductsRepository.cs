using Microsoft.EntityFrameworkCore;

using ModernTenon.Api.Repositories.Contracts;
using ModernTenon.Api.Repositories.Contracts.Records;

namespace ModernTenon.Api.Repositories.Implimentation;

public class ProductsRepository(DatabaseContext context) : BaseRepository(context), IProductsRepository
{
    public async Task<(int size, IEnumerable<ProductRecord> results)> GetListAsync(int page, int limit)
    {
        if (page < 1 || limit < 1)
        {
            throw new ArgumentException("Page and Limit must be positive integers.");
        }

        var size = await _context.Products.CountAsync();

        var offset = (page - 1) * limit;
        var results = await _context.Products
            .OrderBy(p => p.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        return (size, results);
    }

    public async Task<ProductRecord> CreateAsync(ProductRecord record)
    {
        if (record == null)
        {
            throw new ArgumentNullException(nameof(record), "Product record cannot be null.");
        }

        var result = await CommitAsync(async () =>
        {
            var createdRecordEntry = await _context.Products.AddAsync(record);
            return createdRecordEntry.Entity;
        });

        return result;
    }

    public async Task<ProductRecord?> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(id), "Product ID is not valid.");
        }

        var result = await _context.Products.FindAsync(id);
        return result;
    }

    public async Task<ProductRecord> UpdateAsync(ProductRecord record)
    {
        if (record == null)
        {
            throw new ArgumentNullException(nameof(record), "Product record cannot be null.");
        }

        var existingRecord = await GetAsync(record.Id);
        if (existingRecord == null)
        {
            throw new ArgumentNullException($"Product record with ID {record.Id} not found.");
        }

        var result = await CommitAsync(() =>
        {
            _context.Entry(existingRecord).CurrentValues.SetValues(record);

            return Task.FromResult(existingRecord);
        });

        return result;
    }
}
