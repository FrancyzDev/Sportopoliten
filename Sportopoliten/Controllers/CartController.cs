using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportopoliten.BLL.Interfaces;
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

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Json(new { count = 0 });
                }

                int userId = int.Parse(userIdClaim.Value);
                var count = await _cartService.GetCartItemCountAsync(userId);

                return Json(new { count = count });
            }
            catch (Exception ex)
            {
                return Json(new { count = 0, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Json(new { success = false, message = "Пользователь не авторизован" });
                }

                int userId = int.Parse(userIdClaim.Value);
                await _cartService.ClearCartAsync(userId);

                return Json(new { success = true, message = "Корзина очищена" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int count = 1, string? size = null)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Json(new { success = false, redirectToLogin = true, message = "Необходимо авторизоваться" });
                }

                int userId = int.Parse(userIdClaim.Value);
                await _cartService.AddToCartAsync(userId, productId, count, size);

                var totalCount = await _cartService.GetCartItemCountAsync(userId);

                return Json(new { success = true, count = totalCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int count, string? size = null)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Json(new { success = false });

                int userId = int.Parse(userIdClaim.Value);
                await _cartService.UpdateQuantityAsync(userId, productId, count, size);

                var totalCount = await _cartService.GetCartItemCountAsync(userId);
                return Json(new { success = true, count = totalCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId, string? size = null)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Json(new { success = false });

                int userId = int.Parse(userIdClaim.Value);
                await _cartService.RemoveItemAsync(productId, userId, size);

                var totalCount = await _cartService.GetCartItemCountAsync(userId);
                return Json(new { success = true, count = totalCount, message = "Товар удален" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}