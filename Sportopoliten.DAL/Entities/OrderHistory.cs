using System;
using System.Collections.Generic;
using System.Text;

﻿namespace Sportopoliten.DAL.Entities
{
    public class OrderHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TrackingNumber { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = [];
    }
}
