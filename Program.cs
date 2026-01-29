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
        static void Main(string[] args)
        {
            using var context = new MyDbContext();
            while (true)
            {
                User.Users.ChooseCustomer(context);
                while (true)
                {
                    UI.Home.DrawHomePage(context);
                }
            }

        }        
        
    }
}
