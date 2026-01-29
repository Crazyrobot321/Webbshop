using System;
using System.Collections.Generic;
using System.Text;
using Webbshop.Enumerables;

namespace Webbshop.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public required Customer Customer { get; set; }

        public DateTime OrderDate { get; set; }

        public DeliveryOptions DeliveryOption { get; set; }

        public PaymentOptions PaymentMethod { get; set; }

        public string? LastFourDigits { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public required string ShippingName { get; set; }
        public required string ShippingStreet { get; set; }
        public required string ShippingCity { get; set; }
        public required string ShippingCountry { get; set; }

    }

}
