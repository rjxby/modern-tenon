using System.ComponentModel.DataAnnotations;

using ModernTenon.Api.Host.Requests.Product;

namespace ModernTenon.Api.Host.Requests;

public record CreateProductRequest
{
    [Required]
    [MinLength(3, ErrorMessage = ValidationRules.NameMinLengthMessage)]
    public string? Name { get; init; }
}