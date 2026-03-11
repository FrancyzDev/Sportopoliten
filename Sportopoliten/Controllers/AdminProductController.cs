using Microsoft.AspNetCore.Mvc;
using Sportopoliten.BLL.DTO;
using Sportopoliten.ViewModels.Product;
using Sportopoliten.BLL.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

public class AdminProductController : Controller
{
    private readonly IProductService _productService;

    private readonly IWebHostEnvironment _env;

    public AdminProductController(IProductService productService)
    {
        _productService = productService;

    }

    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new CreateProductDTO
        {
            Title = model.Title,
            Description = model.Description,
            CategoryId = model.CategoryId,
            Variants = new List<ProductVariantDTO>()
        };


        foreach (var variant in model.Variants)
        {
            var imageUrls = new List<string>();

            foreach (var image in variant.Images)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);

                var path = Path.Combine(_env.WebRootPath, "images/products", fileName);

                using var stream = new FileStream(path, FileMode.Create);

                await image.CopyToAsync(stream);

                imageUrls.Add("/images/products/" + fileName);
            }
            
            dto.Variants.Add(new ProductVariantDTO
            {
                Color = variant.Color,
                Size = variant.Size,
                Price = variant.Price,
                Stock = variant.Stock,
                Images = imageUrls
            });
        }

        await _productService.CreateProductAsync(dto);

        return RedirectToAction("Index");
    }
}
