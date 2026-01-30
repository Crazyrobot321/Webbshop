using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Webbshop.Entities;
using Webbshop.UI;
using Webbshop.Data;

namespace Webbshop
{
    internal class Program
    {
        public static Dictionary<int, int> cart = new(); // key=ProductId, value=Quantity 
        public static Customer CurrentUser { get; set; }
        public static void Main()
        {
            using var context = new MyDbContext();
            while (true)
            {

                Console.Clear();
                Console.WriteLine("Welcome to the Webshop!");
                Console.WriteLine("1. Login as Existing Customer");
                Console.WriteLine("2. Create New Customer");
                Console.WriteLine("3. Exit");
                Console.Write("Please select an option (1-3): ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        User.Users.ChooseCustomer(context);
                        break;
                    case "2":
                        User.CreateNew.CreateNewCustomer(context);
                        break;
                    case "3":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.ReadKey();
                        continue;
                }
                while (true)
                {
                    UI.Home.DrawHomePage(context);
                }
            }

        }        
        
    }
}
