using Sportopoliten.BLL.DTO;
using Sportopoliten.DAL.Data;
using Sportopoliten.DAL.Entities;

public class ProductService : IProductService
{
    private readonly ShopDbContext _context;

    public ProductService(ShopDbContext context)
    {
        _context = context;
    }

    public async Task CreateProductAsync(CreateProductDTO dto)
    {
        var product = new Product
        {
            Title = dto.Title,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            Variants = new List<ProductVariant>()
        };

        foreach (var variantDto in dto.Variants)
        {
            var variant = new ProductVariant
            {
                Color = variantDto.Color,
                Size = variantDto.Size,
                Price = variantDto.Price,
                Stock = variantDto.Stock,
                Images = new List<ProductVariantImages>()
            };

            foreach (var imageUrl in variantDto.Images)
            {
                variant.Images.Add(new ProductVariantImages
                {
                    ImageUrl = imageUrl
                });
            }

            product.Variants.Add(variant);
        }

        _context.Products.Add(product);

        await _context.SaveChangesAsync();
    }
}