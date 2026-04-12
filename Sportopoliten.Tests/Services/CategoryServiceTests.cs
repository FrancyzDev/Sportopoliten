using Moq;
using Sportopoliten.BLL.DTO.Category;
using Sportopoliten.BLL.Services;
using Sportopoliten.DAL.Entities;
using Sportopoliten.DAL.Interfaces;

namespace Sportopoliten.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IRepository<Category>> _repo = new();

    private CategoryService CreateService()
    {
        _uow.Setup(u => u.Categories).Returns(_repo.Object);
        return new CategoryService(_uow.Object);
    }

    [Fact]
    public async Task GetAllCategories_ReturnsAllItems()
    {
        var categories = new List<Category> { new() { Id = 1 }, new() { Id = 2 } };
        _repo.Setup(r => r.GetWithQueryAsync(It.IsAny<Func<IQueryable<Category>, IQueryable<Category>>>()))
             .ReturnsAsync(categories);

        var result = await CreateService().GetAllCategoriesAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task CreateCategory_SavesAndReturnsCategory()
    {
        var dto = new CreateCategoryDTO { Title = "Футбол", ImageUrl = "img.jpg" };
        _repo.Setup(r => r.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await CreateService().CreateCategoryAsync(dto);

        Assert.Equal("Футбол", result.Title);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCategory_WithProducts_ThrowsInvalidOperation()
    {
        var category = new Category
        {
            Id = 1,
            Products = new List<Product> { new() { Id = 1 } }
        };
        _repo.Setup(r => r.GetSingleWithQueryAsync(It.IsAny<Func<IQueryable<Category>, IQueryable<Category>>>()))
             .ReturnsAsync(category);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => CreateService().DeleteCategoryAsync(1));
    }

    [Fact]
    public async Task UpdateCategory_NotFound_ThrowsKeyNotFound()
    {
        _repo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>()))
             .ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => CreateService().UpdateCategoryAsync(999, new UpdateCategoryDTO { Title = "X" }));
    }
}
