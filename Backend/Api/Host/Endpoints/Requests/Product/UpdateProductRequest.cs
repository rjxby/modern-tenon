using System.ComponentModel.DataAnnotations;

using ModernTenon.Api.Host.Requests.Product;

namespace ModernTenon.Api.Host.Requests;

public record UpdateProductRequest
{
    [Required]
    [MinLength(3, ErrorMessage = ValidationRules.NameMinLengthMessage)]
    public string? Name { get; init; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = ValidationRules.PriceMinValueMessage)]
    public double? Price { get; init; }
}