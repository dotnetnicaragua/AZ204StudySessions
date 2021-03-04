using System;
using System.Collections.Generic;
using System.Text;

namespace SQLApiCosmosDB.Models
{
    class Device
    {
        public int Ram { get; set; }
        public string OperatingSystem { get; set; }
        public int CameraMegaPixels { get; set; }
        public string Usage { get; set; }
    }
}
