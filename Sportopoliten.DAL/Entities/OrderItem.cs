namespace Sportopoliten.DAL.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string? ProductName { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public int Count { get; set; }
        public decimal Subtotal { get; set; }
        public string? Size { get; set; }
    }
}
