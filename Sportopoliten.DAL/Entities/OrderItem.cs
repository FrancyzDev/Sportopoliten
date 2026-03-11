using System;
using System.Collections.Generic;
using System.Text;
﻿namespace Sportopoliten.DAL.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public OrderHistory Order { get; set; } = null!;
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;
        public string? ProductName { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public int Count { get; set; }
        public decimal Subtotal { get; set; }
    }
}
