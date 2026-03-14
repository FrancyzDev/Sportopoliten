namespace Sportopoliten.BLL.DTO.Order
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAdress { get; set; }
        public string PaymentMethod { get; set; }
        public string TrackingNumber { get; set; }
    }
}
