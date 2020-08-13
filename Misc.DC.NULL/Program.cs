// Use this code inside a project created with the Visual C# > Windows Desktop > Console Application template.
// Replace the code in Program.cs with this code.

using Misc.DC.DataSource;
using System;
using System.IO.Ports;
using System.Threading;

public class PortChat
{


    public static void Main()
    {
        bool ok = true;
        //配置serial port
        SerialPort _serialPort = new SerialPort()
        {
            PortName = "COM1",
            StopBits = StopBits.One,
            BaudRate = 9600,
            DataBits = 8,
            Parity = Parity.Odd,
            ReadTimeout = 500,
            WriteTimeout = 500
        };
        SerialPortDataServer serial = new SerialPortDataServer(ok,_serialPort);
        serial.LoadData();

    }

}