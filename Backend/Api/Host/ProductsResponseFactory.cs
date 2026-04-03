using ModernTenon.Api.Services.Contracts.Entities;

namespace ModernTenon.Api.Host;

internal static class ProductsResponseFactory
{
    public static ProductResponse ToResponse(ProductEntity entity)
    {
        return new ProductResponse(entity.Id, entity.Name, entity.Price);
    }

    public static PaginationResponse<ProductResponse> ToPaginationResponse(PaginationResultEntity<ProductEntity> entity)
    {
        return new PaginationResponse<ProductResponse>(
            entity.Page,
            entity.Limit,
            entity.Size,
            entity.Results.Select(ToResponse));
    }
}
