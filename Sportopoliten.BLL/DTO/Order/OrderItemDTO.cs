using System;
using System.Collections.Generic;
using System.Text;

namespace Sportopoliten.BLL.DTO.Order
{
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string PricePurchase { get; set; }
        public int Count { get; set; }
        public int SubTotal { get; set; }
    }
}
