using System;
using System.Linq;
using System.Threading;
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
                    var typeLabel = GetUserTypeLabel(customers[i].UserType);
                    Console.WriteLine($"{i + 1}. {customers[i].Name}, user type: {typeLabel}");
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

        private static string GetUserTypeLabel(object userTypeObj)
        {
            var asString = userTypeObj.ToString();

            //Maps enum names to labels to improve readability
            if (int.TryParse(asString, out int numeric))
            {
                return numeric == 1 ? "Admin" : "User";
            }

            // Fallback to the raw string representation
            return asString = "Unkown";
        }
    }
}
