using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Misc.DC.Models;
using Misc.DC.Storage;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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


        public SerialPortDataServer(bool __continue, MySerialPort serialPort)
        {
            _continue = __continue;
            _serialPort = serialPort;
        }
        //public SerialPortDataServer()
        //{

        //}
        //public bool ConfigSerial(bool __continue, MySerialPort serialPort)
        //{
        //    _continue = __continue;
        //    _serialPort = serialPort;
        //    return true;
        //}

        public void LoadData()
        {
            Thread readThread = new Thread(Read);
            _serialPort.Open();
            // readThread.IsBackground = true;
            readThread.Start();
            readThread.Join();
            _serialPort.Close();
        }

        public static void Read()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
          //  optionsBuilder.UseMySQL("Server=localhost;Database=TempAndHumidSystem;User Id=root;password=123456;port=3306;Charset=utf8;SslMode=none;");

            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var DbString = config.GetSection("ConnectionStrings:DBString").Value;
            optionsBuilder.UseMySQL(DbString);



            using (DcDbContext _dcDbContext = new DcDbContext(optionsBuilder.Options))
            //  using (DcDbContext _dcDbContext = new DcDbContext())
            {
                while (_continue)
                {
                    try
                    {
                        string message = _serialPort.ReadLine();
                        Console.WriteLine(message);

                        //反序列化数据
                        List<string> list = message.Split("=").ToList();

                        //   Dictionary<string, decimal> keyValuePairs = new Dictionary<string, decimal>();
                        decimal res = 0;
                        decimal.TryParse(list[1], out res);
                        // keyValuePairs.Add(list[0], res);

                        string name = list[0];
                        decimal value = res;
                        //读取数据 一份保存在数据库中 一份使用 tcp推送到客户端（轮询即可 不需要实时推送ConcurrentQueue）
                        TempAndHumid tempAndHumid = new TempAndHumid()
                        {
                            addDateTime = System.DateTime.Now,
                            name = name,
                            value = value
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



