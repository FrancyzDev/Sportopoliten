namespace Sportopoliten.DAL.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public OrderStatus? Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TrackingNumber { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = [];
    }
}
