using Microsoft.EntityFrameworkCore;
using Sportopoliten.BLL.DTO;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.DAL.Data;
using Sportopoliten.DAL.Entities;
using Sportopoliten.DAL.Interfaces;
using Sportopoliten.DAL.Repositories;

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

    public async Task UpdateProductAsync(int productId, UpdateProductDTO dto)
    {
        var product = await _context.Products
            .Include(p => p.Variants)
            .ThenInclude(v => v.Images)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            throw new KeyNotFoundException("Товар не найден");
        }

        product.Title = dto.Title;
        product.Description = dto.Description;
        product.CategoryId = dto.CategoryId;

        //product.Variants.Clear();
        _context.ProductVariants.RemoveRange(product.Variants);

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

        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);

        if (product == null)
            throw new KeyNotFoundException("Товар не найден");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}