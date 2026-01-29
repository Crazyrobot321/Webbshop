using System;
using System.Collections.Generic;
using System.Text;
using Webbshop.Data;
using Webbshop.Entities;
using Webbshop.Enumerables;

namespace Webbshop.Purchase
{
    internal class Buy
    {
        public static void Purchase(MyDbContext context,int customerId,PaymentOptions payment,DeliveryOptions delivery,string? lastFourDigits = null)
        {
            if (Program.cart.Count == 0)
            {
                Console.WriteLine("Varukorgen är tom.");
                return;
            }

            using var tx = context.Database.BeginTransaction();

            try
            {
                var productIds = Program.cart.Keys.ToList();

                var products = context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToList();

                //safety check, checks if all products still exist 
                if (products.Count != productIds.Count)
                    throw new Exception("One or more products no longer available.");

                var orderItems = new List<OrderItem>();

                foreach (var product in products)
                {
                    int qty = Program.cart[product.Id];

                    if (qty <= 0)
                        throw new Exception("Invalid quantity.");

                    if (product.Quantity < qty)
                        throw new Exception(
                            $"Insufficent stock for {product.Name}. In stock: {product.Quantity}");

                    if (product.Price == null)
                        throw new Exception($"Product {product.Name} is missing a price.");

                    // minska lagersaldo
                    product.Quantity -= qty;

                    orderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = qty,
                        UnitPrice = product.Price.Value
                    });
                }

                var order = new Order
                {
                    CustomerId = customerId,
                    Customer = Program.CurrentUser,
                    OrderDate = DateTime.Now,
                    PaymentMethod = payment,
                    DeliveryOption = delivery,
                    LastFourDigits = lastFourDigits,

                    ShippingName = Cart.ShippingName,
                    ShippingStreet = Cart.ShippingStreet,
                    ShippingCity = Cart.ShippingCity,
                    ShippingCountry = Cart.ShippingCountry
                };

                context.Orders.Add(order);
                context.SaveChanges(); //Need to save to generate order.Id

                foreach (var oi in orderItems)
                    oi.OrderId = order.Id;

                context.Set<OrderItem>().AddRange(orderItems);
                context.SaveChanges();

                tx.Commit();

                Program.cart.Clear();
                Console.WriteLine("Transaction completed!");
            }
            catch (Exception ex)
            {
                tx.Rollback();
                Console.WriteLine($"Transaction failed: {ex.Message}");
            }
        }

    }
}
