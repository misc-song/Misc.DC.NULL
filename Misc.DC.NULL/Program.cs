// Use this code inside a project created with the Visual C# > Windows Desktop > Console Application template.
// Replace the code in Program.cs with this code.

using Misc.DC.DataSource;
using Misc.DC.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;

public class PortChat
{
    public static void Main(string[] argv)
    {
        Console.WriteLine(argv);
        List<string> ls = argv[0].Split(":").ToList();
        bool ok = true;
        foreach (var i in GetPortNames())
        {
            Console.WriteLine(i);
        }
        //配置serial port
        MySerialPort _serialPort = new MySerialPort()
        {
            guid = System.Guid.NewGuid().ToString(),
            PortName = ls[0],
            StopBits = (StopBits)int.Parse(ls[1]),
            BaudRate = int.Parse(ls[2]),
            DataBits = int.Parse(ls[3]),
            Parity = (Parity)int.Parse(ls[4]),
            ReadTimeout = int.Parse(ls[5]),
            WriteTimeout = int.Parse(ls[6])
        };
        SerialPortDataServer serial = new SerialPortDataServer(ok, _serialPort);
        serial.LoadData();
    }
    public static String[] GetPortNames()
    {
        return SerialPort.GetPortNames();
    }

}