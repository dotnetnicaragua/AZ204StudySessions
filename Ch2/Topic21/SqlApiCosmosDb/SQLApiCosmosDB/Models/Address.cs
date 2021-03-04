using System;
using System.Collections.Generic;
using System.Text;

namespace SQLApiCosmosDB.Models
{
    class Address
    {
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
    }
}
