namespace Sportopoliten.BLL.DTO.Cart
{
    public class CartDTO
    {
        public int Id { get; set; }
        public List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
        public decimal TotalPrice { get; set; }
        public int TotalQuantity => Items?.Sum(x => x.Count) ?? 0;
    }
}
