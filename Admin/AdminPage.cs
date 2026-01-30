using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
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
            while (true)
            {
                Console.Clear();
                var adminText = new List<string>
                {
                "1. Edit Products",
                "2. Edit Categories",
                "3. Edit Users",
                "4. View Statistics",
                "0. Return to Main Menu"
                };
                var adminWindow = new Window("Admin Menu", 40, 10, adminText);
                adminWindow.Draw();
                Console.Write("Choice: ");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Invalid choice!");
                    continue;
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
                        case 0:
                            return;
                        default:
                            break;
                    }
                }

            }
        }
        static void AdministrateCategories(MyDbContext context)
        {
            while (true)
            {
                Console.Clear();
                var windowContent = new List<string>
                {
                    "1. Add category",
                    "2. Edit category",
                    "0. Back"
                };
                new Window("Category Administration", 40, 10, windowContent).Draw();
                Console.Write("Choice: ");
                var input = Console.ReadLine();
                //If input is invalid, continue asking for input again
                if (!int.TryParse(input, out int choice))
                    continue;
                switch (choice)
                {
                    case 1:
                        AddCategory(context);
                        break;
                    case 2:
                        EditCategory(context);
                        break;
                    case 0:
                        return;
                }
            }

        }
        static void AddCategory(MyDbContext context)
        {
            Console.Clear();
            Console.Write("Category name: ");
            var name = Console.ReadLine();
            var category = new Category
            {
                Name = name!
            };
            context.Categories.Add(category);
            context.SaveChanges();
            Console.WriteLine("Category added.");
            Console.ReadKey();
        }
        static void EditCategory(MyDbContext context)
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
            
            var windowContent = new List<string>
            {
                "1. View Order History",
                "2. Edit User Information",
                "0. Back"
            };
            new Window("User Administration", 40, 10, windowContent).Draw();
            Console.Write("Choice: ");
            var input = Console.ReadLine();
            //If input is not a valid integer, return
            if (!int.TryParse(input, out int choice))
                return;
            var customers = context.Customers.ToList();
            
            switch (choice)
            {
                case 1:
                    foreach (var c in customers)
                        Console.WriteLine($"[{c.Id}] {c.Name} | {c.Email}");

                    Console.Write("Customer ID: ");
                    if (!int.TryParse(Console.ReadLine(), out int custId))
                        return;
                    //EntityFramework
                    var orders = context.Orders
                        .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                        .Where(o => o.CustomerId == custId)
                        .ToList();
                    foreach (var o in orders)
                    {
                        //Calculate total price for the order
                        decimal total = 0m;
                        if (o.OrderItems != null && o.OrderItems.Any())
                        {
                            total = o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice);
                        }

                        Console.WriteLine($"Order ID: {o.Id} | Date: {o.OrderDate} | Delivery: {o.DeliveryOption} | Payment: {o.PaymentMethod} | Total: {total:0.00} kr");
                        foreach (var oi in o.OrderItems)
                        {
                            Console.WriteLine($"\tProduct: {oi.Product?.Name} | Qty: {oi.Quantity} | Unit Price: {oi.UnitPrice} kr");
                        }
                    }
                    break;
                case 2:
                    foreach (var c in customers)
                        Console.WriteLine($"[{c.Id}] {c.Name} | {c.Email}");

                    Console.Write("Customer ID: ");
                    if (!int.TryParse(Console.ReadLine(), out int custId2))
                        return;
                    var customer = customers.FirstOrDefault(c => c.Id == custId2);
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
                case 0:
                    return;
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

            var categories = context.Categories.ToList();
            Console.WriteLine("Currently available categories: ");
            foreach (var c in categories)
                Console.WriteLine($"[{c.Id}] {c.Name}");
            Console.Write("Category ID: ");
            if (!int.TryParse(Console.ReadLine(), out int categoryId))
                return;
            var supplier = context.Suppliers.ToList();
            Console.WriteLine("Currently available suppliers: ");
            foreach (var s in supplier)
                Console.WriteLine($"[{s.Id}] {s.Name}");
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
                SupplierId = supplierId,
                IsSelected = false
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
            //Leave field empty to keep current value
            Console.WriteLine("Leave fields empty to keep current values");
            Console.Write($"Name ({product.Name}): ");
            var name = Console.ReadLine();
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

            var categories = context.Categories.ToList();
            Console.WriteLine("Currently available categories: ");
            foreach (var c in categories)
                Console.WriteLine($"[{c.Id}] {c.Name}");
            Console.Write($"Category ID ({product.CategoryId}): ");
            if (int.TryParse(Console.ReadLine(), out int catId))
                product.CategoryId = catId;

            var supplier = context.Suppliers.ToList();
            Console.WriteLine("Currently available suppliers: ");
            foreach (var s in supplier)
                Console.WriteLine($"[{s.Id}] {s.Name}");

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

            Console.Write("Product ID to remove, (or press Enter to return): ");
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
        
        //Dapper based statistics viewing
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
                    "5. Detailed information about orders",
                    "6. Back"
                };
                Console.Clear();
                new Window("Statistics Menu", 50, 12, queries).Draw();
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
                        new Window("Most sold products", 5, 10, mostSoldList).Draw();
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
                        new Window("Top Customers by Purchase Amount", 5, 10, topCustomersList).Draw();
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
                        new Window("Most sold per category", 5, 10, mostSoldByCategoryList).Draw();
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
                        new Window("Orders per customer", 5, 10, ordersPerCustomerList).Draw();
                        Console.ReadKey(true);
                        break;
                    case 5:
                        Console.Clear();
                        var detailedOrderInformation = new List<string> { };
                        var detailedOrders = conn.Query(@"SELECT
                            o.Id AS OrderId,
                            c.Name AS CustomerName,
                            CASE o.PaymentMethod
                                WHEN 1 THEN 'Credit Card'
                                WHEN 2 THEN 'PayPal'
                                WHEN 3 THEN 'Bank Transfer'
                                WHEN 4 THEN 'Klarna'
                                ELSE 'Unknown'
                            END AS PaymentMethod,
                            CASE o.DeliveryOption
                                WHEN 1 THEN 'Standard'
                                WHEN 2 THEN 'Express'
                                WHEN 3 THEN 'In Store PickUp'
                                WHEN 4 THEN 'Drone'
                                ELSE 'Unknown'
                            END AS DeliveryMethod,
                            STRING_AGG(
                                p.Name + ' x' + CAST(oi.Quantity AS varchar(10)),
                                ', '
                            ) AS Products,
                            SUM(oi.Quantity * oi.UnitPrice) AS OrderTotal
                        FROM Orders o
                        JOIN 
                            Customers c ON c.Id = o.CustomerId
                        JOIN 
                            OrderItem oi ON oi.OrderId = o.Id
                        JOIN 
                            Products p ON p.Id = oi.ProductId
                        GROUP BY
                            o.Id,
                            c.Name,
                            o.PaymentMethod,
                            o.DeliveryOption
                        ORDER BY o.Id;");
                        foreach(var row in detailedOrders)
                        {
                            detailedOrderInformation.Add(
                                $"Order ID: {row.OrderId} | Customer: {row.CustomerName} | " +
                                $"Payment: {row.PaymentMethod} | Delivery: {row.DeliveryMethod} | " +
                                $"Products: {row.Products} | Total: {row.OrderTotal} kr"
                            );
                        }
                        new Window("TEST", 1, 1, detailedOrderInformation).Draw();
                        Console.ReadKey(true);
                        return;
                    case 6:
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
