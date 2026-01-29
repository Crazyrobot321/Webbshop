using System;
using System.Collections.Generic;
using System.Text;
using Webbshop.Data;
using Webbshop.Purchase;

namespace Webbshop.UI
{
    internal class Home
    {
        public static void DrawHomePage(MyDbContext context)
        {
            Console.Clear();

            //Welcome text
            var headerText = new List<string>
            {
                $"Welcome {Program.CurrentUser.Name} to Nordic Threads",
                "Stylish clothes for everyday and work",
                "",
                "Choose a product by pressing the corresponding letter"
            };
            new Window("Startpage", 2, 1, headerText).Draw();

            //Current User
            var currentUserText = new List<string>
                {
                $"Logged in as:",
                $"{Program.CurrentUser.Name}",
                $"{Program.CurrentUser.Email}"
            };
            new Window("Customer Information", 65, 1, currentUserText).Draw();
            //Menu
            var customerMenuText = new List<string>
                {
                "1. Startpage",
                "2. The shop",
                "3. Cart",
                "4. Admin Stuff"
            };
            new Window("Menu", 2, 7, customerMenuText).Draw();

            //Fetch products which have IsSelected = true
            var startProducts = context.Products
                .Where(p => p.IsSelected == true)
                .ToList();

            //If no products are available
            if (!startProducts.Any())
            {
                Console.SetCursorPosition(2, 8);
                Console.WriteLine("No products available now.");
                return;
            }

            //Draw smaller version of cart
            Purchase.Cart.DrawSmallCart(context);

            //Featured product windows 
            int startX = 2;
            int startY = 18;
            int windowWidth = 32;

            for (int i = 0; i < startProducts.Count; i++)
            {
                //Translate index to letter, 0 = A, 1 = B, etc, max 26 entries
                if (i >= 26)
                    break;
                char key = (char)('A' + i);
                int x = startX + (i * windowWidth);

                var product = startProducts[i];

                var productText = new List<string>
                {
                    product.Name,
                    $"{product.Price} kr",
                    "",
                    $"Press [{key}] to buy"
                };

                new Window($"Deal {i + 1}",x,startY,productText).Draw();
            }

            //Handling input
            Console.SetCursorPosition(2, startY + 8);
            Console.Write("Input: ");
            var key2 = Console.ReadKey(true).KeyChar;

            if (char.IsLetter(key2))
            {
                char input = char.ToUpper(key2);
                int index = input - 'A';

                if (key2 == 'x')
                {
                    Purchase.Cart.Checkout(context);
                    Console.ReadLine();
                }
                if (index >= 0 && index < startProducts.Count)
                {
                    var chosenProduct = startProducts[index];

                    //If product is already in cart, increase amount otherwise add new entry
                    if (Program.cart.ContainsKey(chosenProduct.Id))
                        Program.cart[chosenProduct.Id] += 1;
                    else
                        Program.cart[chosenProduct.Id] = 1;

                    Console.WriteLine($"{chosenProduct.Name} Added to cart (Amount: {Program.cart[chosenProduct.Id]})");
                }
                else
                {
                    Console.WriteLine("Invalid choice");
                }
            }
            else if (char.IsDigit(key2))
            {
                switch (key2)
                {
                    case '1':
                        //Redraw home page
                        break;
                    case '2':
                        //Draw shop page
                        UI.Shop.DrawShopPage(context);
                        Thread.Sleep(500);
                        break;
                    case '3':
                        //Draw cart page
                        Console.Clear();
                        Purchase.Cart.Checkout(context);
                        Console.WriteLine("Press any key to return.");
                        Console.ReadKey(true);
                        break;
                    case '4':
                        //Draw admin page
                        Admin.AdminPage.DrawAdminPage(context);
                        Console.WriteLine("Press any key to return.");
                        Console.ReadKey(true);
                        break;
                    case '5':
                        break;
                    default:
                        Console.WriteLine("Invalid choice");
                        Thread.Sleep(500);
                        break;
                }
            }
        }
    }
}
