using System;
using System.Collections.Generic;
using System.Text;

namespace Webbshop.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string MobileNr { get; set; }
        public required DateTime DateOfBirth { get; set; }

        //Adressen
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }

    }
}
