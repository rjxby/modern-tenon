using ModernTenon.Api.Host.Requests;
using ModernTenon.Api.Services.Contracts;
using ModernTenon.Api.Services.Contracts.Entities;

namespace ModernTenon.Api.Host;

public static class ProductsEndpoint
{
    public static void MapProductsEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/products")
            .WithTags("Products");
        
        group.MapGet("/", async (IProductService service, [AsParameters] PaginationRequest request) =>
        {
            var paginationEntity = new PaginationEntity(request.Page, request.Limit);
            var result = await service.GetListAsync(paginationEntity);
            return ProductsResponseFactory.ToPaginationResponse(result);
        });

        group.MapGet("{id}", async (IProductService service, Guid id) =>
        {
            var result = await service.GetAsync(id);
            return ProductsResponseFactory.ToResponse(result);
        });

        group.MapPost("/", async (IProductService service, CreateProductRequest request) =>
        {
            var modelToCreate = new ProductEntity(Guid.NewGuid(), request.Name!);
            var result = await service.CreateAsync(modelToCreate);
            return ProductsResponseFactory.ToResponse(result);
        });

        group.MapPut("{id}", async (IProductService service, Guid id, UpdateProductRequest request) =>
        {
            var modelToUpdate = new ProductEntity(id, request.Name!, request.Price!.Value);
            var result = await service.UpdateAsync(id, modelToUpdate);
            return ProductsResponseFactory.ToResponse(result);
        });
    }
}
