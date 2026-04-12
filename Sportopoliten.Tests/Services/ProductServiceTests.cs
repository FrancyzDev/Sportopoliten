using Moq;
using Sportopoliten.BLL.DTO.Product;
using Sportopoliten.BLL.Services;
using Sportopoliten.DAL.Entities;
using Sportopoliten.DAL.Interfaces;

namespace Sportopoliten.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IRepository<Product>> _repo = new();
    private readonly Mock<IRepository<ProductImage>> _imageRepo = new();

    private ProductService CreateService()
    {
        _uow.Setup(u => u.Products).Returns(_repo.Object);
        _uow.Setup(u => u.ProductImages).Returns(_imageRepo.Object);
        return new ProductService(_uow.Object);
    }

    [Fact]
    public async Task GetProductById_Found_ReturnsMappedDTO()
    {
        var product = new Product
        {
            Id = 1, Title = "Мяч", Price = 500m,
            Category = new Category { Title = "Футбол" },
            ProductImages = new List<ProductImage>()
        };
        _repo.Setup(r => r.GetSingleWithQueryAsync(It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>()))
             .ReturnsAsync(product);

        var result = await CreateService().GetProductByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Мяч", result!.Title);
        Assert.Equal("Футбол", result.CategoryName);
    }

    [Fact]
    public async Task GetProductById_NotFound_ReturnsNull()
    {
        _repo.Setup(r => r.GetSingleWithQueryAsync(It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>()))
             .ReturnsAsync((Product?)null);

        var result = await CreateService().GetProductByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProduct_AssignsImagePrioritiesInOrder()
    {
        var dto = new CreateProductDTO
        {
            Title = "Ракетка", Price = 1200m, CategoryId = 1,
            ProductImages = new List<string> { "a.jpg", "b.jpg", "c.jpg" }
        };
        _repo.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await CreateService().CreateProductAsync(dto);

        Assert.Equal(3, result.ProductImages.Count);
        Assert.Equal(0, result.ProductImages[0].Priority);
        Assert.Equal(2, result.ProductImages[2].Priority);
    }

    [Fact]
    public async Task DeleteProduct_NotFound_ThrowsKeyNotFound()
    {
        _repo.Setup(r => r.GetSingleWithQueryAsync(It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>()))
             .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => CreateService().DeleteProductAsync(999));
    }

    [Fact]
    public async Task GetFilteredProducts_ByPrice_ReturnsOnlyMatching()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Title = "Дешёвый", Price = 100m, ProductImages = new() },
            new() { Id = 2, Title = "Средний", Price = 500m, ProductImages = new() },
            new() { Id = 3, Title = "Дорогой", Price = 1000m, ProductImages = new() }
        };
        _repo.Setup(r => r.GetWithQueryAsync(It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>()))
             .ReturnsAsync(products);

        var (result, total) = await CreateService().GetFilteredProductsAsync(
            null, null, 200m, 800m, null, 1, 10);

        Assert.Equal(1, total);
        Assert.Equal("Средний", result.Single().Title);
    }

    [Fact]
    public async Task GetMinMaxPrice_EmptyCatalog_ReturnsDefaults()
    {
        _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>());

        var min = await CreateService().GetMinPriceAsync();
        var max = await CreateService().GetMaxPriceAsync();

        Assert.Equal(0, min);
        Assert.Equal(10000, max);
    }
}
