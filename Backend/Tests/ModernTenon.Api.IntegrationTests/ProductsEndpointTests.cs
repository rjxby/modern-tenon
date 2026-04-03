using System.Net.Http.Json;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
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
        var convertedCents = existedProduct.PriceInCents / 100m;
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
        var request = new UpdateProductRequest { Name = $"{Guid.NewGuid()}", Price = 100m };

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
        var priceInCents = (ulong)(request.Price.Value * 100m);
        existedProduct.PriceInCents.Should().Be(priceInCents);
    }

    [Fact]
    public async Task ExecuteSqlRaw_InvalidProductRow_ThrowsDueToDatabaseConstraint()
    {
        // Arrange
        var invalidName = "ab";

        // Act
        var action = async () =>
        {
            await DbContext.Database.ExecuteSqlAsync($"""
                INSERT INTO Products (Id, Name, PriceInCents)
                VALUES ({Guid.NewGuid()}, {invalidName}, {100UL})
                """);
        };

        // Assert
        await action.Should().ThrowAsync<SqliteException>();
    }

    private static List<ProductRecord> GenerateFakeProducts()
    {
        var productFaker = new Faker<ProductRecord>()
            .RuleFor(p => p.Id, f => Guid.NewGuid())
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.PriceInCents, f => (ulong)(decimal.Round(f.Random.Decimal(10, 1000), 2) * 100m));

        return productFaker.Generate(3);
    }
}
