using AutoMapper;
using ModernTenon.Api.Services.Contracts.Entities;

namespace ModernTenon.Api.Host;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProductEntity, ProductResponse>();

        CreateMap(typeof(PaginationResultEntity<>), typeof(PaginationResponse<>));
    }
}
