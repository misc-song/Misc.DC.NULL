using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Misc.DC.Models;
using Misc.DC.Storage;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Misc.DC.DataSource
{
    public class SerialPortDataServer
    {
        public static bool _continue { get; set; }
        public static SerialPort _serialPort { get; set; }


        public SerialPortDataServer(bool __continue, SerialPort serialPort, DcDbContext dcDbContext)
        {
            _continue = __continue;
            _serialPort = serialPort;
        }
        public void LoadData()
        {
            Thread readThread = new Thread(Read);
            _serialPort.Open();
            readThread.Start();
            readThread.Join();
            _serialPort.Close();
        }

        public static void Read()
        {
            //DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            //optionsBuilder.UseMySQL("Server=localhost;Database=OASystem2;User Id=root;password=123456;port=3306;Charset=utf8;SslMode=none;");
            //using (DcDbContext _dcDbContext = new DcDbContext(optionsBuilder.Options))
            using (DcDbContext _dcDbContext = new DcDbContext())
            {
                while (_continue)
                {
                    try
                    {
                        string message = _serialPort.ReadLine();
                        Console.WriteLine(message);

                        //反序列化数据
                        List<string> list = message.Split(",").ToList();

                        decimal humid = Decimal.Parse(list[0]);
                        decimal temp = Decimal.Parse(list[1]);

                        TempAndHumid tempAndHumid = new TempAndHumid()
                        {
                            addDateTime = System.DateTime.Now,
                            humidity = humid,
                            temperature = temp
                        };

                        _dcDbContext.Add(tempAndHumid);
                        _dcDbContext.SaveChanges();


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("出现异常{0}", ex.Message);
                    }
                }
            }

        }

        //停止服务
        public static void StopRead()
        {
            _continue = false;
        }
    }
}



