namespace Sportopoliten.BLL.DTO.Cart
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public string ImageUrl { get; set; }
        public decimal TotalPrice => Price * Count;

    }
}
