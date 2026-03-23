using Sportopoliten.BLL.DTO.Order;

namespace Sportopoliten.BLL.DTO
{
    public class CreateOrderDTO
    {
        public int UserId { get; set; }

        public string ShippingAddress { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public List<OrderItemDTO> Items { get; set; } = new();
    }
}
