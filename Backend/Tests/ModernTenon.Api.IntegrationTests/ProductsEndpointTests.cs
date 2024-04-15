using System.Net.Http.Json;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

using ModernTenon.Api.Host;
using ModernTenon.Api.Repositories.Contracts.Records;
using ModernTenon.Api.Repositories.Implimentation;
using ModernTenon.Api.Host.Requests;

namespace ModernTenon.Api.IntegrationTests;

public class ProductsEndpointTests : TestFixture<Program, DatabaseContext>
{
    protected override async Task SeedDatabaseAsync(DatabaseContext context)
    {
        var seedingProducts = GenerateFakeProducts();
        await context.Products.AddRangeAsync(seedingProducts);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetList_Products_ReturnsOk()
    {
        // Arrange
        var existedProductsCount = await DbContext.Products.CountAsync();
        var expectedPage = 1;
        var expectedLimit = existedProductsCount;

        // Act
        var response = await HttpClient.GetFromJsonAsync<PaginationResponse<ProductResponse>>($"/api/products?page={expectedPage}&limit={expectedLimit}");

        // Assert
        response.Should().NotBeNull();
        response!.Size.Should().Be(expectedLimit);
        response!.Page.Should().Be(expectedPage);
        response.Results.Count().Should().Be(existedProductsCount);
    }

    [Fact]
    public async Task Get_ExistingProduct_ReturnsOk()
    {
        // Arrange
        var existedProduct = await DbContext.Products.FirstAsync();

        // Act
        var response = await HttpClient.GetFromJsonAsync<ProductResponse>($"/api/products/{existedProduct.Id}/");

        // Assert
        response.Should().NotBeNull();
        response!.Id.Should().Be(existedProduct.Id);
        response.Name.Should().Be(existedProduct.Name);
        var convertedCents = existedProduct.PriceInCents / 100;
        response.Price.Should().Be(convertedCents);
    }

    [Fact]
    public async Task Create_NewProduct_ReturnsOk()
    {
        // Arrange
        var request = new CreateProductRequest { Name = "test name" };

        // Act
        var responseContent = await HttpClient.PostAsJsonAsync($"/api/products/", request);
        var response = await responseContent.Content.ReadFromJsonAsync<ProductResponse>();

        // Assert
        response.Should().NotBeNull();
        response!.Id.Should().NotBe(Guid.Empty);
        response.Name.Should().Be(request.Name);

        var createdRecord = await DbContext.Products.FirstOrDefaultAsync(p => p.Id == response.Id);
        createdRecord!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task Update_Product_ReturnsOk()
    {
        // Arrange
        var existedProduct = await DbContext.Products.FirstAsync();
        var request = new UpdateProductRequest { Name = $"{Guid.NewGuid()}", Price = 100 };

        // Act
        var responseContent = await HttpClient.PutAsJsonAsync($"/api/products/{existedProduct.Id}/", request);
        var response = await responseContent.Content.ReadFromJsonAsync<ProductResponse>();

        // Assert
        response.Should().NotBeNull();
        response!.Id.Should().Be(existedProduct.Id);
        response.Name.Should().Be(request.Name);
        response.Price.Should().Be(request.Price);

        await DbContext.Entry(existedProduct).ReloadAsync();
        existedProduct.Name.Should().Be(request.Name);
        var priceInCents = (ulong)(request.Price.Value * 100);
        existedProduct.PriceInCents.Should().Be(priceInCents);
    }

    private static List<ProductRecord> GenerateFakeProducts()
    {
        var productFaker = new Faker<ProductRecord>()
            .RuleFor(p => p.Id, f => Guid.NewGuid())
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.PriceInCents, f => (ulong)Math.Round(f.Random.Double(10, 1000), 2) * 100);

        return productFaker.Generate(3);
    }
}
