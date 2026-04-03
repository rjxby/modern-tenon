using System.ComponentModel.DataAnnotations;

using ModernTenon.Api.Host.Requests.Product;

namespace ModernTenon.Api.Host.Requests;

public record UpdateProductRequest
{
    [Required]
    [MinLength(3, ErrorMessage = ValidationRules.NameMinLengthMessage)]
    public string? Name { get; init; }

    [Required]
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = ValidationRules.PriceMinValueMessage)]
    public decimal? Price { get; init; }
}
