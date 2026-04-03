using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.ViewModels.HomeViewModels;
using System.Security.Claims;

namespace Sportopoliten.Controllers
{
    public class CartController : Controller
    {

        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null)
            {
                int userId = int.Parse(userIdClaim.Value);

                var cart = await _cartService.GetCartAsync(userId);
                return View(cart);
            }

            return RedirectToAction("Login", "Account");
        }
    }
}
