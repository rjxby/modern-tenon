namespace ModernTenon.Api.Services.Contracts.Entities;

public record PaginationResultEntity<T>(int Page, int Limit, int Size, IEnumerable<T> Results) : PaginationEntity(Page, Limit)
{
}
