namespace ModernTenon.Api.Host;

public record PaginationResponse<T>(int Page, int Limit, int Size, IEnumerable<T> Results)
{
}
