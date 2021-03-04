using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLApiCosmosDB.Models
{
    class Person
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Device[] Devices { get; set; }
        public Address Address { get; set; }
        public string Gender { get; set; }
        public bool IsRegistered { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
