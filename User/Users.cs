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
                Console.WriteLine("Välj kund:");
                for (int i = 0; i < customers.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {customers[i].Name}");
                }
                Console.Write("Ange kundnummer: ");
                if (int.TryParse(Console.ReadLine(), out int customerIndex) && customerIndex >= 1 && customerIndex <= customers.Count)
                {
                    var selectedCustomer = customers[customerIndex - 1];
                    Console.WriteLine($"Vald kund: {selectedCustomer.Name}");
                    Program.CurrentUser = selectedCustomer;
                    break; //Breaks the loop when a valid customer is selected
                }
                else
                {
                    Console.WriteLine("Ogiltigt kundnummer.");
                    Thread.Sleep(800);
                }
            }
        }
    }
}
