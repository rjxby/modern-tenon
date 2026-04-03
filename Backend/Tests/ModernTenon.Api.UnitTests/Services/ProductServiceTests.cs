using Bogus;
using Moq;
using Xunit;
using FluentAssertions;

using ModernTenon.Api.Repositories.Contracts;
using ModernTenon.Api.Repositories.Contracts.Records;
using ModernTenon.Api.Services.Contracts.Entities;
using ModernTenon.Api.Services.Implimentation.Services;

namespace ModernTenon.Api.UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IProductsRepository> _mockProductsRepository;
    private readonly ProductService _service;
    private readonly Faker<ProductRecord> _productRecordFaker;
    private readonly Faker<ProductEntity> _productEntityFaker;

    public ProductServiceTests()
    {
        _mockProductsRepository = new Mock<IProductsRepository>();
        _service = new ProductService(_mockProductsRepository.Object);

        _productRecordFaker = new Faker<ProductRecord>()
            .RuleFor(o => o.Id, f => f.Random.Guid())
            .RuleFor(o => o.Name, f => f.Commerce.ProductName())
            .RuleFor(o => o.PriceInCents, f => f.Random.ULong(10, 1000));



        _productEntityFaker = new Faker<ProductEntity>()
            .CustomInstantiator(f => new ProductEntity(
                f.Random.Guid(),
                f.Commerce.ProductName(),
                decimal.Round(f.Random.Decimal(10, 1000), 2)));
    }

    [Fact]
    public async Task GetListAsync_Returns_PaginationResultEntityWithProducts()
    {
        // Arrange
        var expectedSize = 10;
        var pagination = new PaginationEntity(1, expectedSize);
        var productRecords = _productRecordFaker.Generate(expectedSize);

        _mockProductsRepository.Setup(repo => repo.GetListAsync(pagination.Page, pagination.Limit))
            .ReturnsAsync((expectedSize, productRecords));

        // Act
        var result = await _service.GetListAsync(pagination);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PaginationResultEntity<ProductEntity>>();
        result.Results.Should().HaveCount(expectedSize);
        result.Results.Should().BeEquivalentTo(productRecords.Select(record =>
            record.PriceInCents.HasValue
                ? new ProductEntity(record.Id, record.Name, record.PriceInCents.Value / 100m)
                : new ProductEntity(record.Id, record.Name)));
        _mockProductsRepository.Verify(repo => repo.GetListAsync(pagination.Page, pagination.Limit), Times.Once);
    }

    [Fact]
    public async Task GetAsync_Returns_ProductEntity()
    {
        // Arrange
        var id = Guid.NewGuid();
        var productRecord = _productRecordFaker.Generate();

        _mockProductsRepository.Setup(repo => repo.GetAsync(id))
            .ReturnsAsync(productRecord);

        // Act
        var result = await _service.GetAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ProductEntity>();
        result.Should().BeEquivalentTo(
            productRecord.PriceInCents.HasValue
                ? new ProductEntity(productRecord.Id, productRecord.Name, productRecord.PriceInCents.Value / 100m)
                : new ProductEntity(productRecord.Id, productRecord.Name));
        _mockProductsRepository.Verify(repo => repo.GetAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenProductNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockProductsRepository.Setup(repo => repo.GetAsync(id)).Returns(Task.FromResult<ProductRecord?>(null));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetAsync(id));
        exception.Message.Should().Be($"Product with ID {id} not found.");
    }

    [Fact]
    public async Task CreateAsync_NewRecordReturns_ProductEntity()
    {
        // Arrange
        var productEntityToCreate = _productEntityFaker.Generate();
        var createdProductRecord = _productRecordFaker.Generate();
        var expectedRecordToCreate = new ProductRecord
        {
            Id = productEntityToCreate.Id,
            Name = productEntityToCreate.Name,
            PriceInCents = productEntityToCreate.Price.HasValue
                ? checked((ulong)(productEntityToCreate.Price.Value * 100m))
                : null
        };

        _mockProductsRepository.Setup(repo => repo.CreateAsync(It.Is<ProductRecord>(record =>
                record.Id == expectedRecordToCreate.Id &&
                record.Name == expectedRecordToCreate.Name &&
                record.PriceInCents == expectedRecordToCreate.PriceInCents)))
            .ReturnsAsync(createdProductRecord);

        // Act
        var result = await _service.CreateAsync(productEntityToCreate);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ProductEntity>();
        result.Should().BeEquivalentTo(
            createdProductRecord.PriceInCents.HasValue
                ? new ProductEntity(createdProductRecord.Id, createdProductRecord.Name, createdProductRecord.PriceInCents.Value / 100m)
                : new ProductEntity(createdProductRecord.Id, createdProductRecord.Name));
        _mockProductsRepository.Verify(repo => repo.CreateAsync(It.Is<ProductRecord>(record =>
            record.Id == expectedRecordToCreate.Id &&
            record.Name == expectedRecordToCreate.Name &&
            record.PriceInCents == expectedRecordToCreate.PriceInCents)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatedRecordReturns_ProductEntity()
    {
        // Arrange
        var productEntityToUpdate = _productEntityFaker.Generate();
        var productRecordToUpdate = _productRecordFaker.Generate();
        var updatedProductRecord = _productRecordFaker.Generate();

        _mockProductsRepository.Setup(repo => repo.GetAsync(productEntityToUpdate.Id))
            .ReturnsAsync(productRecordToUpdate);
        _mockProductsRepository.Setup(repo => repo.UpdateAsync(productRecordToUpdate))
            .ReturnsAsync(updatedProductRecord);

        // Act
        var result = await _service.UpdateAsync(productEntityToUpdate.Id, productEntityToUpdate);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ProductEntity>();
        result.Should().BeEquivalentTo(
            updatedProductRecord.PriceInCents.HasValue
                ? new ProductEntity(updatedProductRecord.Id, updatedProductRecord.Name, updatedProductRecord.PriceInCents.Value / 100m)
                : new ProductEntity(updatedProductRecord.Id, updatedProductRecord.Name));
        productRecordToUpdate.Name.Should().Be(productEntityToUpdate.Name);
        productRecordToUpdate.PriceInCents.Should().Be(
            productEntityToUpdate.Price.HasValue
                ? checked((ulong)(productEntityToUpdate.Price.Value * 100m))
                : null);
        _mockProductsRepository.Verify(repo => repo.GetAsync(productEntityToUpdate.Id), Times.Once);
        _mockProductsRepository.Verify(repo => repo.UpdateAsync(productRecordToUpdate), Times.Once);
    }

    [Fact]
    public void ProductEntity_WithMoreThanTwoDecimals_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var action = () => new ProductEntity(Guid.NewGuid(), "Valid Name", 10.999m);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*cannot have more than 2 decimal places*");
    }

    [Fact]
    public void ProductEntity_WithShortName_ThrowsArgumentException()
    {
        // Act
        var action = () => new ProductEntity(Guid.NewGuid(), "ab");

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*at least 3 characters long*");
    }

    [Fact]
    public void ProductRecord_WithZeroPriceInCents_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var action = () => new ProductRecord
        {
            Id = Guid.NewGuid(),
            Name = "Valid Name",
            PriceInCents = 0
        };

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*must be greater than zero*");
    }
}
