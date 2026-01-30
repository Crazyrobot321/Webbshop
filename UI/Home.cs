using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using Webbshop.Data;
using Webbshop.Purchase;

namespace Webbshop.UI
{
    internal class Home
    {
        public static bool changeAdr = false;
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

            //Menu - show admin option only for admins
            var customerMenuText = new List<string>
            {
                "1. Startpage",
                "2. The shop",
                "3. Cart",
                "4. Change user / logout",
                "5. Change adress",
            };

            if (Program.CurrentUser != null && Program.CurrentUser.UserType == 1)
            {
                customerMenuText.Add("6. Admin Stuff");
            }

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

                new Window($"Deal {i + 1}", x, startY, productText).Draw();
            }

            //Handling input
            Console.SetCursorPosition(2, startY + 8);
            Console.Write("Input: ");
            var key2 = Console.ReadKey(true).KeyChar;

            if (char.IsLetter(key2))
            {
                char input = char.ToUpper(key2);
                int index = input - 'A';

                if (char.ToUpper(key2) == 'X')
                {
                    Purchase.Cart.Checkout(context);
                    Console.ReadLine();
                }
                else if (index >= 0 && index < startProducts.Count)
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
                        Console.Clear();
                        Program.cart.Clear();
                        Program.Main();
                        break;
                    case '5':
                        Purchase.Cart.once = false;
                        Purchase.Cart.UpdateShippingInfo(Purchase.Cart.once);
                        Purchase.Cart.once = true;
                        break;
                    case '6':
                        //Only allow admin page for admins
                        if (Program.CurrentUser != null && Program.CurrentUser.UserType == 1)
                        {
                            Admin.AdminPage.DrawAdminPage(context);
                            Thread.Sleep(500);
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice, only available for admins");
                            Thread.Sleep(500);
                        }
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
