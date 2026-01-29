using System;
using System.Collections.Generic;
using System.Text;
using Webbshop.Data;

namespace Webbshop.User
{
    internal class Users
    {
        public static void ChooseCustomer(MyDbContext context)
        {
            var customers = context.Customers.ToList();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose customer:");
                for (int i = 0; i < customers.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {customers[i].Name}");
                }
                Console.Write("Input CustomerID: ");
                if (int.TryParse(Console.ReadLine(), out int customerIndex) && customerIndex >= 1 && customerIndex <= customers.Count)
                {
                    var selectedCustomer = customers[customerIndex - 1];
                    Console.WriteLine($"Chosen customer: {selectedCustomer.Name}");
                    Program.CurrentUser = selectedCustomer;
                    break; //Breaks the loop when a valid customer is selected
                }
                else
                {
                    Console.WriteLine("Invalid CustomerID.");
                    Thread.Sleep(800);
                }
            }
        }
    }
}
