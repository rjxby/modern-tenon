namespace ModernTenon.Api.Host.Requests;

using System.ComponentModel.DataAnnotations;

public record PaginationRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
    public int Page { get; init; }

    [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100.")]
    public int Limit { get; init; }
}

