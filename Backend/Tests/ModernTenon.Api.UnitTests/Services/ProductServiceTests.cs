using AutoMapper;
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
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductService _service;
    private readonly Faker<ProductRecord> _productRecordFaker;
    private readonly Faker<ProductEntity> _productEntityFaker;

    public ProductServiceTests()
    {
        _mockProductsRepository = new Mock<IProductsRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new ProductService(_mockMapper.Object, _mockProductsRepository.Object);

        _productRecordFaker = new Faker<ProductRecord>()
            .RuleFor(o => o.Id, f => f.Random.Guid())
            .RuleFor(o => o.Name, f => f.Commerce.ProductName())
            .RuleFor(o => o.PriceInCents, f => f.Random.ULong(10, 1000));



        _productEntityFaker = new Faker<ProductEntity>()
            .CustomInstantiator(f => new ProductEntity(
                f.Random.Guid(),
                f.Commerce.ProductName(),
                f.Random.Double(10, 1000)));
    }

    [Fact]
    public async Task GetListAsync_Returns_PaginationResultEntityWithProducts()
    {
        // Arrange
        var expectedSize = 10;
        var pagination = new PaginationEntity(1, expectedSize);
        var productRecords = _productRecordFaker.Generate(expectedSize);
        var productEntities = _productEntityFaker.Generate(expectedSize);

        _mockProductsRepository.Setup(repo => repo.GetListAsync(pagination.Page, pagination.Limit))
            .ReturnsAsync((expectedSize, productRecords));
        _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ProductEntity>>(It.IsAny<IEnumerable<ProductRecord>>()))
            .Returns(productEntities);

        // Act
        var result = await _service.GetListAsync(pagination);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PaginationResultEntity<ProductEntity>>();
        result.Results.Should().HaveCount(expectedSize);
        _mockProductsRepository.Verify(repo => repo.GetListAsync(pagination.Page, pagination.Limit), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<IEnumerable<ProductEntity>>(It.IsAny<IEnumerable<ProductRecord>>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_Returns_ProductEntity()
    {
        // Arrange
        var id = Guid.NewGuid();
        var productRecord = _productRecordFaker.Generate();
        var productEntity = _productEntityFaker.Generate();

        _mockProductsRepository.Setup(repo => repo.GetAsync(id))
            .ReturnsAsync(productRecord);
        _mockMapper.Setup(mapper => mapper.Map<ProductEntity>(It.IsAny<ProductRecord>()))
            .Returns(productEntity);

        // Act
        var result = await _service.GetAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ProductEntity>();
        _mockProductsRepository.Verify(repo => repo.GetAsync(id), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<ProductEntity>(productRecord), Times.Once);
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
        var productRecordToCreate = _productRecordFaker.Generate();
        var createdProductRecord = _productRecordFaker.Generate();
        var createdProductEntity = _productEntityFaker.Generate();

        _mockMapper.Setup(mapper => mapper.Map<ProductRecord>(productEntityToCreate))
            .Returns(productRecordToCreate);
        _mockProductsRepository.Setup(repo => repo.CreateAsync(productRecordToCreate))
            .ReturnsAsync(createdProductRecord);
        _mockMapper.Setup(mapper => mapper.Map<ProductEntity>(createdProductRecord))
            .Returns(createdProductEntity);

        // Act
        var result = await _service.CreateAsync(productEntityToCreate);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ProductEntity>();
        _mockMapper.Verify(mapper => mapper.Map<ProductRecord>(productEntityToCreate), Times.Once);
        _mockProductsRepository.Verify(repo => repo.CreateAsync(productRecordToCreate), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<ProductEntity>(createdProductRecord), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatedRecordReturns_ProductEntity()
    {
        // Arrange
        var productEntityToUpdate = _productEntityFaker.Generate();
        var productRecordToUpdate = _productRecordFaker.Generate();
        var updatedProductRecord = _productRecordFaker.Generate();
        var updatedProductEntity = _productEntityFaker.Generate();

        _mockProductsRepository.Setup(repo => repo.GetAsync(productEntityToUpdate.Id))
            .ReturnsAsync(productRecordToUpdate);
        _mockMapper.Setup(mapper => mapper.Map(productEntityToUpdate, productRecordToUpdate))
            .Returns(productRecordToUpdate);
        _mockProductsRepository.Setup(repo => repo.UpdateAsync(productRecordToUpdate))
            .ReturnsAsync(updatedProductRecord);
        _mockMapper.Setup(mapper => mapper.Map<ProductEntity>(updatedProductRecord))
            .Returns(updatedProductEntity);

        // Act
        var result = await _service.UpdateAsync(productEntityToUpdate.Id, productEntityToUpdate);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ProductEntity>();
        _mockProductsRepository.Verify(repo => repo.GetAsync(productEntityToUpdate.Id), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map(productEntityToUpdate, productRecordToUpdate), Times.Once);
        _mockProductsRepository.Verify(repo => repo.UpdateAsync(productRecordToUpdate), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<ProductEntity>(updatedProductRecord), Times.Once);
    }
}
