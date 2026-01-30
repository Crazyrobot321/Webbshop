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
            while (true)
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
                    continue;
                }
                foreach (var category in categories)
                {
                    categoryText.Add($"[{categories.IndexOf(category) + 1}]. {category.Name}");
                }
                categoryText.Add("[S]. Search products");
                categoryText.Add("[B]. To go back");
                new Window("Product Categories", 2, 1, categoryText).Draw();
                Console.SetCursorPosition(1, categoryText.Count + 3);
                Console.WriteLine("Select a number corresponding to the category, or press Enter to cancel");
                Console.Write("Choice: ");
                var line = Console.ReadLine().ToUpper();
                line.Trim();

                //Free text search option
                if (line.Equals("S"))
                {
                    Search(context);
                }
                if(line.Equals("B"))
                {
                    return;
                }

                // Check for invalid input by trying to parse to int and checking range
                if (!int.TryParse(line, out int catIndex) || catIndex < 1 || catIndex > categories.Count)
                {
                    Console.WriteLine("Invalid choice");
                    Thread.Sleep(500);
                    continue;
                }

                var selectedCategory = categories[catIndex - 1];

                // Using sql to fetch products in selected category
                var products = context.Products
                    .Where(p => p.CategoryId == selectedCategory.Id)
                    .ToList();

                Console.Clear();
                if (!products.Any())
                {
                    Console.WriteLine("No products in selected category");
                    Console.WriteLine("Press any button to return...");
                    Console.ReadKey(true);
                    continue;
                }

                // Show products with numeric keys as options
                for (int i = 0; i < products.Count; i++)
                {
                    var prod = products[i];
                    productText.Add($"[{i + 1}]. {prod.Name} - {prod.Price} kr");
                }
                new Window("Products", 2, 1, productText).Draw();
                Console.WriteLine();
                Console.Write("Choose which product to learn more about: ");
                string inputRaw = Console.ReadLine() ?? string.Empty;
                string input = inputRaw.Trim().ToUpperInvariant();
                // Check for empty input and handle in Results
                Results(input, products);
                Thread.Sleep(500);
                continue;
            }
        }
        static void Results(string input, List<Product> products)
        {
            input = (input ?? string.Empty).Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Invalid choice");
                Thread.Sleep(500);
                return;
            }
            if (input.Equals("B", StringComparison.Ordinal))
            {
                return;
            }
            if (int.TryParse(input, out int prodSelection) && prodSelection >= 1 && prodSelection <= products.Count)
            {
                Console.Clear();
                var chosen = products[prodSelection - 1];
                var prodDetail = new List<string>
                {
                    $"Name: {chosen.Name}",
                    $"Price: {chosen.Price} kr",
                    $"Description: {chosen.Description}"
                };
                new Window("Product Details", 2, 1, prodDetail).Draw();
                Console.Write("Add to cart? Y/N: ");
                var confirmKey = Console.ReadKey(true).KeyChar;
                if (char.ToUpperInvariant(confirmKey) != 'Y')
                {
                    Console.WriteLine("Purchase cancelled, returning...");
                    Thread.Sleep(500);
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
        static void Search(MyDbContext context)
        {
            Console.Clear();
            Console.WriteLine("Type 'exit' to exit search");
            Console.Write("Free text search: ");
            var searchTerm = Console.ReadLine()?.Trim() ?? string.Empty;
            if (searchTerm.IndexOf("exit", StringComparison.OrdinalIgnoreCase) >= 0)
                return;
            var searchResults = context.Products
             .Where(p =>
                 p.Name.Contains(searchTerm) ||
                 p.Description.Contains(searchTerm))
             .ToList();
            Console.Clear();
            Console.WriteLine($"Products matching search ({searchTerm})");
            var foundProducts = new List<string>();
            foreach (var prod in searchResults)
            {
                foundProducts.Add($"[{prod.Id}] {prod.Name}: {prod.Description} - {prod.Price} kr");
            }
            foundProducts.Add("[B] Go back");
            new Window("Search result", 2, 1, foundProducts).Draw();
            Console.Write("Choose product: ");
            string choiceRaw = Console.ReadLine() ?? string.Empty;
            string choice = choiceRaw.Trim().ToUpperInvariant();
            Results(choice, searchResults);
        }
    }
}
