using Moq;
using Sportopoliten.BLL.Services;
using Sportopoliten.DAL.Entities;
using Sportopoliten.DAL.Interfaces;

namespace Sportopoliten.Tests.Services;

public class CartServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IRepository<Cart>> _cartRepo = new();
    private readonly Mock<IRepository<CartItem>> _itemRepo = new();
    private readonly Mock<IRepository<Product>> _productRepo = new();
    private readonly Mock<IRepository<ProductImage>> _imageRepo = new();

    private CartService CreateService()
    {
        _uow.Setup(u => u.Carts).Returns(_cartRepo.Object);
        _uow.Setup(u => u.CartItems).Returns(_itemRepo.Object);
        _uow.Setup(u => u.Products).Returns(_productRepo.Object);
        _uow.Setup(u => u.ProductImages).Returns(_imageRepo.Object);
        return new CartService(_uow.Object);
    }

    [Fact]
    public async Task GetCart_NoCartExists_ReturnsEmptyCart()
    {
        _cartRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cart, bool>>>()))
                 .ReturnsAsync(new List<Cart>());

        var result = await CreateService().GetCartAsync(userId: 1);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalPrice);
    }

    [Fact]
    public async Task AddToCart_ProductNotFound_Throws()
    {
        _productRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<Exception>(
            () => CreateService().AddToCartAsync(userId: 1, productId: 99, count: 1));
    }

    [Fact]
    public async Task AddToCart_ExistingItem_IncreasesCount()
    {
        var product = new Product { Id = 5, Price = 300m };
        var cart = new Cart { Id = 10, UserId = 1 };
        var existingItem = new CartItem { CartId = 10, ProductId = 5, Count = 2 };

        _productRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(product);
        _cartRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cart, bool>>>()))
                 .ReturnsAsync(new List<Cart> { cart });
        _itemRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<CartItem, bool>>>()))
                 .ReturnsAsync(new List<CartItem> { existingItem });
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await CreateService().AddToCartAsync(userId: 1, productId: 5, count: 3);

        Assert.Equal(5, existingItem.Count);
    }

    [Fact]
    public async Task UpdateQuantity_ZeroCount_DeletesItem()
    {
        var item = new CartItem { ProductId = 5, Count = 2 };
        _itemRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<CartItem, bool>>>()))
                 .ReturnsAsync(new List<CartItem> { item });
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await CreateService().UpdateQuantityAsync(productId: 5, userId: 1, count: 0);
    }

    [Fact]
    public async Task ClearCart_CartExists_DeletesCart()
    {
        var cart = new Cart { Id = 10, UserId = 1 };
        _cartRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cart, bool>>>()))
                 .ReturnsAsync(new List<Cart> { cart });
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await CreateService().ClearCartAsync(userId: 1);
    }
}