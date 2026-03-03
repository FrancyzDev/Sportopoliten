using System;
using System.Collections.Generic;
using System.Text;

namespace Sportopoliten.BLL.DTO
{
    public class CartDTO
    {
        public int Id { get; set; }

        public List<CartItemDTO> Items { get; set; }

        public decimal TotalPrice { get; set; }
        public int TotalQuantity { get; set; }
    }
}
