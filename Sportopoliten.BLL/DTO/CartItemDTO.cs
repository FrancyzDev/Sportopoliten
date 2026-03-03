using System;
using System.Collections.Generic;
using System.Text;

namespace Sportopoliten.BLL.DTO
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductVariantId { get; set; }
        public int Count { get; set; }


    }
}
