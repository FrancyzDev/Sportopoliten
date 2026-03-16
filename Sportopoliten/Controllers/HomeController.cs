using Microsoft.AspNetCore.Mvc;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.ViewModels.HomeViewModels;

namespace Sportopoliten.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryService _categoryService;

        public HomeController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                Categories = await _categoryService.GetAllCategoriesAsync(),
            };

            return View(viewModel);
        }
    }
}