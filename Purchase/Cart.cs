using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webbshop.Data;
using Webbshop.Enumerables;
using Webbshop.UI;

namespace Webbshop.Purchase
{
    internal class Cart
    {
        public static decimal shippingCost = 0m;
        public static string? shippingMethod = "";

        public static string? ShippingName = null;
        public static string? ShippingStreet = null;
        public static string? ShippingCity = null;
        public static string? ShippingCountry = null;
        public static bool once = false;

        public static void DrawSmallCart(MyDbContext context)
        {
            var lines = new List<string>();

            if (Program.cart.Count == 0)
            {
                lines.Add("Cart is empty");
                lines.Add("");
            }
            else
            {
                var productIds = Program.cart.Keys.ToList();

                var products = context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToList();

                decimal grandTotal = 0m;
                int row = 1;

                foreach (var p in products)
                {
                    int qty = Program.cart[p.Id];
                    decimal unit = p.Price ?? 0m;
                    decimal sum = unit * qty;
                    grandTotal += sum;

                    lines.Add($"{row}. {p.Name} x{qty} - {sum} kr ({unit} kr/st)");
                    row++;
                }

                lines.Add("");
                lines.Add($"Totalt: {grandTotal} kr");
                lines.Add("");
                lines.Add("Press X to check out");

            }

            new Window("Cart", 65, 8, lines).Draw();
        }

        public static void DrawCart(MyDbContext context, decimal shipping)
        {
            var lines = new List<string>();
            var productIds = Program.cart.Keys.ToList();

            var products = context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToList();

            decimal total = 0m;
            int row = 1;

            foreach (var p in products)
            {
                int qty = Program.cart[p.Id];
                //Handle products with unavailable price by skipping calculation and showing message instead
                if (!p.Price.HasValue)
                {
                    lines.Add($"{row}. {p.Name} - price unavailable");
                    row++;
                    continue;
                }
                decimal unit = p.Price.Value;
                decimal sum = unit * qty;
                total += sum;

                lines.Add($"{row}. {p.Name} x{qty} - {sum} kr ({unit} kr/st)");
                row++;
            }

            lines.Add("");
            lines.Add($"Wares: {total} kr");
            lines.Add($"Shipping: {shipping} kr");
            lines.Add($"Total: {total + shipping} kr");
            lines.Add("R = Remove item | C = Change quantity");

            new Window("Cart", 2, 8, lines).Draw();
        }

        public static PaymentOptions ChoosePayment(MyDbContext context, decimal shipping, out string? lastFour)
        {
            lastFour = null;
            while (true)
            {
                Console.Clear();
                DrawCart(context, shipping);

                var lines = new List<string>
                {
                    "Choose payment method:",
                    "",
                    "1. Credit Card",
                    "2. PayPal",
                    "3. Bank Transfer",
                    "4. Klarna",
                    "",
                    "B = Go back"
                };

                new Window("Payment", 60, 8, lines).Draw();

                if(!int.TryParse(Console.ReadLine(), out int key))
                {
                    Console.WriteLine("Invalid choice!");
                    continue;
                }

                if (key == 1)
                {
                    Console.SetCursorPosition(2, 28);
                    Console.Write("Enter the last 4 digits of the card: ");
                    lastFour = Console.ReadLine();
                    return PaymentOptions.CreditCard;
                }
                if (key == 2)
                    return PaymentOptions.PayPal;
                if (key == 3)
                    return PaymentOptions.BankTransfer;
                if (key == 4)
                    return PaymentOptions.Klarna;
            }
        }
        public static DeliveryOptions ChooseDelivery(MyDbContext context, ref decimal shipping, PaymentOptions payment)
        {
            while (true)
            {

                Console.Clear();

                DrawCart(context, shipping);

                var lines = new List<string>
                {
                    "Choose delivery method:",
                    "",
                    "1. Standard (49 kr)",
                    "2. Express  (99 kr)",
                    "3. Pickup   (0 kr)",
                    "4. Drone    (199 kr)",
                    "",
                    "B = Go back"
                };

                new Window("Shipping", 60, 8, lines).Draw();

                if (!int.TryParse(Console.ReadLine(), out int key))
                {
                    Console.WriteLine("Invalid choice!");
                    continue;
                }

                //Price assignment to shipping methods
                if (key == 1) {
                    shipping = 49m; 
                    shippingCost = shipping;
                    shippingMethod = "Standard";

                    return DeliveryOptions.Standard; }
                if (key == 2) {
                    shipping = 99m; 
                    shippingCost = shipping;
                    shippingMethod = "Express";

                    return DeliveryOptions.Express; }
                if (key == 3) {
                    shipping = 0m; 
                    shippingCost = shipping;
                    shippingMethod = "InStorePickup";

                    return DeliveryOptions.InStorePickup; }
                if (key == 4) {
                    shipping = 199m; 
                    shippingCost = shipping;
                    shippingMethod = "Drone";

                    return DeliveryOptions.Drone; }
            }
        }
        public static void Checkout(MyDbContext context)
        {
            PaymentOptions payment = PaymentOptions.BankTransfer;
            DeliveryOptions delivery = DeliveryOptions.InStorePickup;
            decimal shipping = 49m;
            string? lastFour = null;

            while (true)
            {
                Console.Clear();
                DrawCart(context, shipping);
                UpdateShippingInfo(once);
                once = true;
                Console.Clear();

                var currentAddress = new List<String>
                {
                    "Shipping to:",
                    $"{ShippingName}",
                    $"{ShippingStreet}",
                    $"{ShippingCity}",
                    $"{ShippingCountry}"
                };
                new Window("Shipping Address", 60, 1, currentAddress).Draw();

                var lines = new List<string>
                {
                    $"Payment: {payment}",
                    $"Shipping: {delivery}",
                    "",
                    "1 = Payment",
                    "2 = Shipping",
                    "X = Purchase",
                    "B = Return",
                    "A = Adjust adress"
                };
                DrawCart(context, shipping);
                new Window("Checkout", 60, 8, lines).Draw();
                Console.Write("Input: ");
                var key = Console.ReadKey(true).Key;
                var productIdsInCart = Program.cart.Keys.OrderBy(id => id).ToList();

                //Using ConsoleKey due to handling both letters and numbers
                switch (key)
                {
                    case ConsoleKey.R:
                        {
                            Console.Write("\nSelect row number to remove product: ");
                            if (!int.TryParse(Console.ReadLine(), out int row) || row < 1 || row > productIdsInCart.Count)
                                break;

                            int productId = productIdsInCart[row - 1];
                            Program.cart.Remove(productId);
                            break;
                        }

                    case ConsoleKey.C:
                        {
                            Console.Write("\nSelect row number to adjust product: ");
                            if (!int.TryParse(Console.ReadLine(), out int row) || row < 1 || row > productIdsInCart.Count)
                                break;

                            Console.Write("\nNew amount: ");
                            if (!int.TryParse(Console.ReadLine(), out int newQty) || newQty < 1)
                                break;

                            int productId = productIdsInCart[row - 1];
                            Program.cart[productId] = newQty;
                            break;
                        }
                }

                if (key == ConsoleKey.B)
                    return;
                if (key == ConsoleKey.A)
                    once = false;
                    UpdateShippingInfo(once);
                    once = true;

                if (key == ConsoleKey.D1)
                    payment = ChoosePayment(context, shipping,out lastFour);

                if (key == ConsoleKey.D2)
                    delivery = ChooseDelivery(context, ref shipping, payment);

                if (key == ConsoleKey.X)
                {
                    Buy.Purchase(context, Program.CurrentUser.Id, payment, delivery);
                    Program.cart.Clear();
                    return;
                }
            }
        }
        public static void UpdateShippingInfo(bool once)
        {
            if(once)
                return;
            Console.WriteLine("Shipping details (press enter to keep default adress)\n");

            Console.Write($"Name ({Program.CurrentUser.Name}): ");
            var shipName = Console.ReadLine();
            ShippingName = string.IsNullOrWhiteSpace(shipName) ? Program.CurrentUser.Name : shipName;

            Console.Write($"Street ({Program.CurrentUser.Street}): ");
            var shipStreet = Console.ReadLine();
            ShippingStreet = string.IsNullOrWhiteSpace(shipStreet) ? Program.CurrentUser.Street : shipStreet;

            Console.Write($"City ({Program.CurrentUser.City}): ");
            var shipCity = Console.ReadLine();
            ShippingCity = string.IsNullOrWhiteSpace(shipCity) ? Program.CurrentUser.City : shipCity;

            Console.Write($"Country ({Program.CurrentUser.Country}): ");
            var shipCountry = Console.ReadLine();
            ShippingCountry = string.IsNullOrWhiteSpace(shipCountry) ? Program.CurrentUser.Country : shipCountry;
            once = true;
        }
    }
}
