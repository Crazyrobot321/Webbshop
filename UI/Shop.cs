using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Webbshop.Entities;
using Webbshop.UI;
using Webbshop.Data;

namespace Webbshop.UI
{
    internal class Shop
    {
        public static void DrawShopPage(MyDbContext context)
        {
            Console.Clear();
            var categoryText = new List<string> { };
            var productText = new List<string> { };
            var categories = context.Categories.ToList();
            if (!categories.Any())
            {
                Console.WriteLine("No categories available.");
                Console.WriteLine("Press any key to return");
                Console.ReadKey(true);
                return;
            }
            for (int i = 0; i < categories.Count; i++)
            {
                categoryText.Add($"{i + 1}. {categories[i].Name}");
            }
            categoryText.Add("[S]. Search products");
            var categoryWindow = new Window("Product Categories", 2, 1, categoryText);
            categoryWindow.Draw();
            Console.SetCursorPosition(1, categoryText.Count + 3);
            Console.WriteLine("Select a number corresponding to the category, or press Enter to cancel");
            Console.Write("Choice: ");
            var line = Console.ReadLine().ToUpper();
            line.Trim();

            //Free text search option
            if (line.Equals("S"))
            {
                Console.Clear();
                Console.Write("Free text search: ");
                var searchTerm = Console.ReadLine()?.Trim() ?? "";

                var searchResults = context.Products
                 .Where(p =>
                     p.Name.Contains(searchTerm) ||
                     p.Description.Contains(searchTerm))
                 .ToList();

                Console.WriteLine($"Products matching search ({searchTerm})");
                foreach (var product in searchResults)
                    Console.WriteLine($"{product.Name}: {product.Description} - {product.Price} kr");
                Console.WriteLine("Press any button to return...");
                Console.ReadKey(true);
            }
            //Check for invalid input by trying to parse to int and checking range
            if (!int.TryParse(line.Trim(), out int catIndex) || catIndex < 1 || catIndex > categories.Count)
            {
                Console.WriteLine("Invalid choice");
                Thread.Sleep(500);
                return;
            }
            

            var selectedCategory = categories[catIndex - 1];

            //Using sql to fetch products in selected category
            var products = context.Products
                .FromSql($"SELECT * FROM Products WHERE CategoryId = {selectedCategory.Id}")
                .ToList();

            Console.Clear();
            if (!products.Any())
            {
                Console.WriteLine("No products in selected category");
                Console.WriteLine("Press any button to return...");
                Console.ReadKey(true);
                return;
            }

            // Show products with numeric keys as options
            for (int i = 0; i < products.Count; i++)
            {
                var prod = products[i];
                productText.Add($"[{i + 1}]. {prod.Name} - {prod.Price} kr");
            }
            var productWindow = new Window("Products", 2, 1, productText);
            productWindow.Draw();

            Console.WriteLine();
            Console.WriteLine("Choose which product to learn more about: ");
            string input = Console.ReadLine().ToUpper();
            //Check for empty input
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Invalid choice");
                Thread.Sleep(500);
                return;
            }

            input = input.Trim();
            if (input.Equals("B"))
            {
                return;
            }
            if (int.TryParse(input, out int prodSelection) && prodSelection >= 1 && prodSelection <= products.Count)
            {
                Console.Clear();
                var chosen = products[prodSelection - 1];
                Console.WriteLine($"{chosen.Name}\n{chosen.Description}");
                Console.WriteLine("Add to cart? Y/N: ");
                var confirmKey = Console.ReadKey(true).KeyChar;
                if (char.ToUpper(confirmKey) != 'Y')
                {
                    Console.WriteLine("Purchase cancelled, returning...");
                    Console.ReadKey(true);
                    return;
                }
                Program.cart[chosen.Id] = Program.cart.GetValueOrDefault(chosen.Id) + 1;
                Console.WriteLine($"{chosen.Name} added to cart");
                Console.WriteLine("Press any button to continue");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("Invalid choice");
            Thread.Sleep(500);
        }
    }
}
