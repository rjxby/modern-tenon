using AutoMapper;

using ModernTenon.Api.Repositories.Contracts.Records;
using ModernTenon.Api.Services.Contracts.Entities;

namespace ModernTenon.Api.Services.Implimentation;

public class MappingProfile : Profile
{
    private const double Cents = 100;

    public MappingProfile()
    {
        CreateMap<ProductEntity, ProductRecord>()
            .ForMember(x => x.PriceInCents, opt => opt.MapFrom(src => src.Price * Cents));

        CreateMap<ProductRecord, ProductEntity>()
            .ConvertUsing(src => src.PriceInCents.HasValue
                ? new ProductEntity(src.Id, src.Name, src.PriceInCents.Value / Cents)
                : new ProductEntity(src.Id, src.Name));
    }
}
