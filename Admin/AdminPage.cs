using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using Webbshop.Data;
using Webbshop.Entities;
using Webbshop.UI;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Webbshop.Admin
{
    internal class AdminPage
    {
        public static void DrawAdminPage(MyDbContext context)
        {
            Console.Clear();
            var adminText = new List<string>
            {
                "1. Edit Products",
                "2. Edit Categories",
                "3. Edit Users",
                "4. View Statistics"
            };
            var adminWindow = new Window("Admin Menu", 40, 10, adminText);
            adminWindow.Draw();
            Console.Write("Choice: ");
            var input = Console.ReadLine();
            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Invalid choice!");
                return;
            }
            else
            {
                Console.Clear();
                switch (choice)
                {
                    case 1:
                        AdministrateProducts(context);
                        break;
                    case 2:
                        AdministrateCategories(context);
                        break;
                    case 3:
                        AdministrateUsers(context);
                        break;
                    case 4:
                        ViewStatistics(context);
                        break;
                    default:
                        break;
                }
            }
        }
        static void AdministrateCategories(MyDbContext context)
        {
            Console.Clear();
            var categories = context.Categories.ToList();
            foreach (var c in categories)
                Console.WriteLine($"[{c.Id}] {c.Name}");

            Console.Write("Category ID: ");
            if (!int.TryParse(Console.ReadLine(), out int catId))
                return;

            var category = categories.FirstOrDefault(c => c.Id == catId);
            if (category == null)
                return;

            Console.Write($"New name ({category.Name}): ");
            var newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName))
                category.Name = newName;

            context.SaveChanges();
            Console.WriteLine("Category updated.");
            Console.ReadKey();

        }
        static void AdministrateUsers(MyDbContext context)
        {
            

            Console.WriteLine("1. View Order History");
            Console.WriteLine("2. Edit User Information");
            Console.Write("Choice: ");
            var input = Console.ReadLine();
            //If input is not a valid integer, return
            if (!int.TryParse(input, out int choice))
                return;
            var customers = context.Customers.ToList();
            foreach (var c in customers)
                Console.WriteLine($"[{c.Id}] {c.Name} | {c.Email}");

            Console.Write("Customer ID: ");
            if (!int.TryParse(Console.ReadLine(), out int custId))
                return;
            switch (choice)
            {
                case 1:
                    //EntityFramework
                    var orders = context.Orders
                        .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                        .Where(o => o.CustomerId == custId)
                        .ToList();
                    foreach (var o in orders)
                    {
                        Console.WriteLine($"Order ID: {o.Id} | Date: {o.OrderDate} | Delivery: {o.DeliveryOption} | Payment: {o.PaymentMethod}");
                        foreach (var oi in o.OrderItems)
                        {
                            Console.WriteLine($"\tProduct: {oi.Product?.Name} | Qty: {oi.Quantity} | Unit Price: {oi.UnitPrice} kr");
                        }
                    }
                    break;
                case 2:
                    var customer = customers.FirstOrDefault(c => c.Id == custId);
                    if (customer == null)
                        return;

                    Console.Write($"Name [{customer.Name}] (leave blank to keep current value): ");
                    var name = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(name))
                        customer.Name = name;

                    Console.Write($"Email [{customer.Email}] (leave blank to keep current value): ");
                    var email = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(email))
                        customer.Email = email;

                    Console.Write($"Mobile [{customer.MobileNr}] (leave blank to keep current value): ");
                    var phone = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(phone))
                        customer.MobileNr = phone;

                    Console.Write($"Street [{customer.Street}] (leave blank to keep current value): ");
                    var street = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(street))
                        customer.Street = street;

                    Console.Write($"City [{customer.City}] (leave blank to keep current value): ");
                    var city = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(city))
                        customer.City = city;

                    Console.Write($"Country [{customer.Country}] (leave blank to keep current value): ");
                    var country = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(country))
                        customer.Country = country;

                    context.SaveChanges();
                    Console.WriteLine("Customer updated.");
                    Console.ReadKey();
                    break;
                default:
                    break;
            }

            
        }
        static void AdministrateProducts(MyDbContext context)
        {
            while (true)
            {
                Console.Clear();
                var windowContent = new List<string>
                {
                    "1. Add product",
                    "2. Edit product",
                    "3. Remove product",
                    "0. Back"
                };
                var adminWindow = new Window("Product Administration", 40, 10, windowContent);
                adminWindow.Draw();

                Console.Write("Choice: ");
                var input = Console.ReadLine();

                //If input is invalid, continue asking for input again
                if (!int.TryParse(input, out int choice))
                    continue;

                switch (choice)
                {
                    case 1:
                        AddProduct(context);
                        break;
                    case 2:
                        EditProduct(context);
                        break;
                    case 3:
                        RemoveProduct(context);
                        break;
                    case 0:
                        return;
                }
            }
        }
        static void AddProduct(MyDbContext context)
        {
            Console.Clear();
            Console.Write("Name: ");
            var name = Console.ReadLine();

            Console.Write("Description: ");
            var desc = Console.ReadLine();

            Console.Write("Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                return;

            Console.Write("Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int qty))
                return;

            Console.Write("Category ID: ");
            if (!int.TryParse(Console.ReadLine(), out int categoryId))
                return;

            Console.Write("Supplier ID: ");
            if (!int.TryParse(Console.ReadLine(), out int supplierId))
                return;

            var product = new Product
            {
                Name = name!,
                Description = desc!,
                Price = price,
                Quantity = qty,
                CategoryId = categoryId,
                SupplierId = supplierId
            };

            context.Products.Add(product);
            context.SaveChanges();

            Console.WriteLine("Product added.");
            Console.ReadKey();
        }
        static void EditProduct(MyDbContext context)
        {
            Console.Clear();
            //EntityFramework
            var products = context.Products
                .Include(p => p.Supplier)
                .Include(p => p.Category)
                .ToList();

            foreach (var p in products)
            {
                Console.WriteLine(
                    $"[{p.Id}] {p.Name} | {p.Price} kr | Qty: {p.Quantity} | " +
                    $"Supplier: {p.Supplier?.Name} | Category: {p.Category?.Name} " +
                    $"Is promoted: {p.IsSelected}"
                );
            }

            //If prodct id is invalid, return
            Console.Write("Product ID: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
                return;
            //If product not found, return
            var product = products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                Console.WriteLine("Product not found");
                Console.ReadKey();
                return;
            }

            Console.Write($"Name ({product.Name}): ");
            var name = Console.ReadLine();
            //If name is not empty, update product name
            if (!string.IsNullOrWhiteSpace(name))
                product.Name = name;

            Console.Write($"Description ({product.Description}): ");
            var desc = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(desc))
                product.Description = desc;

            Console.Write($"Price ({product.Price}): ");
            if (decimal.TryParse(Console.ReadLine(), out decimal price))
                product.Price = price;

            Console.Write($"Quantity ({product.Quantity}): ");
            if (int.TryParse(Console.ReadLine(), out int qty))
                product.Quantity = qty;

            Console.Write($"Featured / IsSelected ({product.IsSelected}) [true/false]: ");
            var featuredInput = Console.ReadLine();
            if (bool.TryParse(featuredInput, out bool isSelected))
                product.IsSelected = isSelected;

            Console.Write($"Category ID ({product.CategoryId}): ");
            if (int.TryParse(Console.ReadLine(), out int catId))
                product.CategoryId = catId;

            Console.Write($"Supplier ID ({product.SupplierId}): ");
            if (int.TryParse(Console.ReadLine(), out int supId))
                product.SupplierId = supId;

            context.SaveChanges();
            Console.WriteLine("Product updated.");
        }
        static void RemoveProduct(MyDbContext context)
        {
            Console.Clear();
            var products = context.Products.ToList();
            foreach (var p in products)
                Console.WriteLine($"[{p.Id}] {p.Name}");

            Console.Write("Product ID to remove: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
                return;

            var product = context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return;

            context.Products.Remove(product);
            context.SaveChanges();

            Console.WriteLine("Product removed.");
            Console.ReadKey();

        }
        
        //Dapper/SQL based statistics viewing
        static void ViewStatistics(MyDbContext context)
        {
            using var conn = new SqlConnection(context.Database.GetConnectionString());
            conn.Open();
            while (true)
            {
                var queries = new List<string>
                {
                    "1. Most sold products",
                    "2. Top customers by purchase amount",
                    "3. Most sold ordered by category",
                    "4. Orders per customer",
                    "5. Back"
                };
                Console.Clear();
                var statsWindow = new Window("Statistics Menu", 50, 12, queries);
                statsWindow.Draw();
                Console.Write("Choice: ");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out int choice))
                    continue;
                switch (choice)
                {
                    case 1:
                        Console.Clear();
                        var mostSoldList = new List<string>{};
                        var mostSold = conn.Query(@"
                            SELECT TOP 10
                                p.Name,
                                SUM(oi.Quantity) AS TotalSold
                            FROM OrderItem oi
                            JOIN Products p ON p.Id = oi.ProductId
                            GROUP BY p.Name
                            ORDER BY TotalSold DESC
                        ");

                        foreach (var row in mostSold)
                        {
                            mostSoldList.Add($"{row.Name} – Sold: {row.TotalSold}");
                        }
                        var mostSoldStatWindow = new Window("Most sold products", 5, 10, mostSoldList);
                        mostSoldStatWindow.Draw();
                        Console.ReadKey(true);
                        break;
                    case 2:
                        Console.Clear();
                        var topCustomersList = new List<string>{};
                        var topCustomers = conn.Query(@"
                            SELECT TOP 10
                                c.Name,
                                SUM(oi.Quantity * oi.UnitPrice) AS TotalSpent
                            FROM Orders o
                            JOIN Customers c ON c.Id = o.CustomerId
                            JOIN OrderItem oi ON oi.OrderId = o.Id
                            GROUP BY c.Name
                            ORDER BY TotalSpent DESC
                        ");

                        foreach (var row in topCustomers)
                        {
                            topCustomersList.Add($"{row.Name} – Spent: {row.TotalSpent} kr");
                        }
                        var topCustomersStatWindow = new Window("Top Customers by Purchase Amount", 5, 10, topCustomersList);
                        topCustomersStatWindow.Draw();
                        Console.ReadKey(true);
                        break;
                    case 3:
                        Console.Clear();
                        var mostSoldByCategoryList = new List<string>{};
                        var mostSoldByCategory = conn.Query(@"
                            SELECT
                                cat.Name AS Category,
                                SUM(oi.Quantity) AS TotalSold
                            FROM OrderItem oi
                            JOIN Products p ON p.Id = oi.ProductId
                            JOIN Categories cat ON cat.Id = p.CategoryId
                            GROUP BY cat.Name
                            ORDER BY TotalSold DESC
                        ");

                        foreach (var row in mostSoldByCategory)
                        {
                            mostSoldByCategoryList.Add($"{row.Category} – Sold: {row.TotalSold}");
                        }
                        var mostSoldByCategoryStatWindow = new Window("Most sold per category", 5, 10, mostSoldByCategoryList);
                        mostSoldByCategoryStatWindow.Draw();
                        Console.ReadKey(true);
                        break;
                    case 4:
                        Console.Clear();
                        var ordersPerCustomerList = new List<string>{};
                        var ordersPerCustomer = conn.Query(@"
                            SELECT TOP 10
                                c.Name,
                                COUNT(o.Id) AS OrderCount
                            FROM Customers c
                            LEFT JOIN Orders o ON o.CustomerId = c.Id
                            GROUP BY c.Name
                            ORDER BY OrderCount DESC
                        ");

                        foreach (var row in ordersPerCustomer)
                        {
                            ordersPerCustomerList.Add($"{row.Name} – Orders: {row.OrderCount}");
                        }
                        var ordersPerCustomerStatWindow = new Window("Orders per customer", 5, 10, ordersPerCustomerList);
                        ordersPerCustomerStatWindow.Draw();
                        Console.ReadKey(true);
                        break;
                    case 5:
                        return;
                    default:
                        break;
                }
                //Close the connection after exiting the loop
                conn.Close();

            }
        }
    }

}
