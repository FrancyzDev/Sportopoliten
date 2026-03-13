using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.DAL.Entities;

namespace AspNetCore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Лучше использовать [controller] вместо жесткого "Categories"
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ICategoryService categoryService,
            ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            try
            {
                // Явно указываем тип и используем правильный синтаксис
                var category = await _categoryService.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    return NotFound($"Категория с ID {id} не найдена");
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении категории с ID {Id}", id);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}