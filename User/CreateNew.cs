using System;
using System.Collections.Generic;
using System.Text;
using Webbshop.Data;
using Webbshop.Entities;

namespace Webbshop.User
{
    internal class CreateNew
    {
        public static void CreateNewCustomer(MyDbContext context)
        {
            Console.Clear();
            Console.WriteLine("Create New Customer\n");

            string ReadRequired(string label)
            {
                string? value;
                do
                {
                    Console.Write($"{label}: ");
                    value = Console.ReadLine()?.Trim();
                }
                while (string.IsNullOrWhiteSpace(value));

                return value!;
            }

            var name = ReadRequired("Full Name");
            var email = ReadRequired("Email");
            var mobileNr = ReadRequired("Mobile Number");

            DateTime dateOfBirth;
            do
            {
                Console.Write("Date of Birth (yyyy-mm-dd): ");
            }
            while (!DateTime.TryParse(Console.ReadLine(), out dateOfBirth));

            var street = ReadRequired("Street");
            var city = ReadRequired("City");
            var country = ReadRequired("Country");

            var newCustomer = new Customer
            {
                Name = name,
                Email = email,
                MobileNr = mobileNr,
                DateOfBirth = dateOfBirth,
                Street = street,
                City = city,
                Country = country,
                UserType = 0
            };

            context.Customers.Add(newCustomer);
            context.SaveChanges();

            Console.WriteLine($"\nCustomer '{name}' created successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            Program.CurrentUser = newCustomer;
            UI.Home.DrawHomePage(context);
        }

    }
}
