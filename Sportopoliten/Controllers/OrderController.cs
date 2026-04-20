using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportopoliten.BLL.DTO;
using Sportopoliten.BLL.DTO.Order;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.DAL.Entities;
using Sportopoliten.ViewModels.OrderViewModels;
using System.Security.Claims;

namespace Sportopoliten.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IUserService _userService;

        public OrderController(IOrderService orderService, ICartService cartService, IUserService userService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            var userCart = await _cartService.GetCartAsync(int.Parse(userIdClaim));
            if (userCart == null || userCart.Items == null || !userCart.Items.Any())
                return RedirectToAction("Index", "Cart");

            return View(userCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder(CheckoutViewModel model)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            int currentUserId = int.Parse(userIdClaim);
            var currentCart = await _cartService.GetCartAsync(currentUserId);

            if (currentCart == null || currentCart.Items == null || !currentCart.Items.Any())
                return RedirectToAction("Index", "Cart");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                }
                return View("Checkout", currentCart);
            }

            var createOrderDto = new CreateOrderDTO
            {
                UserId = currentUserId,
                Items = currentCart.Items.Select(item => new OrderItemDTO
                {
                    ProductId = item.ProductId,
                    Count = item.Count,
                    Size = item.Size
                }).ToList(),
                FullName = model.FullName,
                Phone = model.Phone,
                Email = model.Email,
                City = model.City,
                Address = model.Address,
                PostalCode = model.PostalCode,
                Country = model.Country,
                DeliveryMethod = model.DeliveryMethod,
                PaymentMethod = model.PaymentMethod,
                Comment = model.Comment
            };

            try
            {
                Console.WriteLine($"Попытка создания заказа. UserId: {currentUserId}, Items: {currentCart.Items.Count}");

                var order = await _orderService.CreateOrderAsync(createOrderDto);
                Console.WriteLine($"Заказ создан с ID: {order.Id}");

                await _cartService.ClearCartAsync(currentUserId);

                TempData["SuccessMessage"] = "Заказ успешно оформлен!";
                return RedirectToAction("Success", new { orderId = order.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Произошла ошибка при оформлении заказа: " + ex.Message;
                return View("Checkout", currentCart);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Success(int orderId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null || order.UserId != int.Parse(userIdClaim))
                return NotFound();

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            var orders = await _orderService.GetOrdersByUserIdAsync(int.Parse(userIdClaim));
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.UserId != int.Parse(userIdClaim))
                return NotFound();

            return View(order);
        }
    }
}