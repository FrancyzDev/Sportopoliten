using System;
using System.Collections.Generic;
using System.Text;
﻿namespace Sportopoliten.DAL.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;
        public int Count { get; set; }
    }
}
