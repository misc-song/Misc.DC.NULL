using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Misc.DC.Models;
using Misc.DC.Storage;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Concurrent;
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
        public static DbContextOptionsBuilder _optionsBuilder { get; set; }
        private static ConcurrentQueue<TempAndHumid> _concurrentQueue { get; set; }
        public SerialPortDataServer(bool __continue, MySerialPort serialPort, ConcurrentQueue<TempAndHumid> concurrentQueue, DbContextOptionsBuilder optionsBuilder)
        {
            _continue = __continue;
            _serialPort = serialPort;
            _concurrentQueue = concurrentQueue;
        }

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
            using (DcDbContext _dcDbContext = new DcDbContext(_optionsBuilder.Options))
            {
                while (_continue)
                {
                    try
                    {
                        string message = _serialPort.ReadLine();
                        Console.WriteLine(message);

                        List<string> list = message.Split("=").ToList();
                        decimal res = 0;
                        decimal.TryParse(list[1], out res);

                        string name = list[0];
                        decimal value = res;
                        //读取数据 一份保存在数据库中 一份使用 tcp推送到客户端（轮询即可 钱没给到位不需要实时推送ConcurrentQueue）
                        TempAndHumid tempAndHumid = new TempAndHumid()
                        {
                            addDateTime = System.DateTime.Now,
                            name = name,
                            value = value
                        };
                        _dcDbContext.Add(tempAndHumid);
                        _dcDbContext.SaveChanges();
                        _concurrentQueue.Enqueue(tempAndHumid);
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



