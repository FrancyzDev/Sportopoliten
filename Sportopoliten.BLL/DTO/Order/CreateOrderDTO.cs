using Sportopoliten.BLL.DTO.Order;

namespace Sportopoliten.BLL.DTO
{
    public class CreateOrderDTO
    {
        public int UserId { get; set; }

        public string ShippingAddress { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public List<OrderItemDTO> Items { get; set; } = new();
        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string DeliveryMethod { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;
    }
}