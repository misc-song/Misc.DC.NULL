// Use this code inside a project created with the Visual C# > Windows Desktop > Console Application template.
// Replace the code in Program.cs with this code.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Misc.DC.DataSource;
using Misc.DC.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Threading;

public class PortChat
{
    public static void Main(string[] argv)
    {
        Console.WriteLine(argv);
        DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var DbString = config.GetSection("ConnectionStrings:DBString").Value;
        optionsBuilder.UseMySQL(DbString);

        var ipAddress = config.GetSection("IPEndpoint:IPAdress").Value;
        var port = config.GetSection("IPEndpoint:port").Value;
        //启用线程安全队列
        ConcurrentQueue<TempAndHumid> tempAndHumids = new ConcurrentQueue<TempAndHumid>();

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
        SerialPortDataServer serial = new SerialPortDataServer(ok, _serialPort, tempAndHumids, optionsBuilder);
        serial.LoadData();
        SocketServer socketServer = new SocketServer(ok, IPAddress.Parse(ipAddress), int.Parse(port), tempAndHumids);
        socketServer.Start();
    }
    public static String[] GetPortNames()
    {
        return SerialPort.GetPortNames();
    }

}