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

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int count)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Json(new { success = false });

                int userId = int.Parse(userIdClaim.Value);
                await _cartService.UpdateQuantityAsync(userId, productId, count);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Json(new { success = false });

                int userId = int.Parse(userIdClaim.Value);
                await _cartService.RemoveItemAsync(productId, userId);

                return Json(new { success = true, message = "Товар видалено" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
