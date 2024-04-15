using AutoMapper;

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
        
        group.MapGet("/", async (IMapper mapper, IProductService service, [AsParameters] PaginationRequest request) =>
        {
            var paginationEntity = new PaginationEntity(request.Page, request.Limit);
            var result = await service.GetListAsync(paginationEntity);
            return mapper.Map<PaginationResponse<ProductResponse>>(result);
        });

        group.MapGet("{id}", async (IMapper mapper, IProductService service, Guid id) =>
        {
            var result = await service.GetAsync(id);
            return mapper.Map<ProductResponse>(result);
        });

        group.MapPost("/", async (IMapper mapper, IProductService service, CreateProductRequest request) =>
        {
            var modelToCreate = new ProductEntity(Guid.NewGuid(), request.Name!);
            var result = await service.CreateAsync(modelToCreate);
            return mapper.Map<ProductResponse>(result);
        });

        group.MapPut("{id}", async (IMapper mapper, IProductService service, Guid id, UpdateProductRequest request) =>
        {
            var modelToUpdate = new ProductEntity(id, request.Name!, request.Price!.Value);
            var result = await service.UpdateAsync(id, modelToUpdate);
            return mapper.Map<ProductResponse>(result);
        });
    }
}
