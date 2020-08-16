using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;

namespace Misc.DC.Models
{
    public class MySerialPort : SerialPort
    {
        public string guid { get; set; }
    }
}
