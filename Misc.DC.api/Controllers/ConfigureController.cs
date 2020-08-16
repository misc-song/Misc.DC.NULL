using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Misc.DC.api.Models;
using Misc.DC.DataSource;
using Misc.DC.Models;
using Misc.DC.Storage;

namespace Misc.DC.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigureController : ControllerBase
    {
        public static bool _continue { get; set; }
        private MySerialPort _mySerialPort { get; set; }
        //   private SerialPortDataServer _serialPortDataServer { get; set; }
        private DcDbContext _dcDbContext { get; set; }

        public ConfigureController(MySerialPort mySerialPort,/* SerialPortDataServer serialPortDataServer,*/ DcDbContext dcDbContext)
        {
            _continue = true;
            _mySerialPort = mySerialPort;
            //  _serialPortDataServer = serialPortDataServer;
            _dcDbContext = dcDbContext;
        }
        //检查进程是否存在
        [HttpGet("CheckProcessExist")]
        public IActionResult CheckProcessExist()
        {
            return null;
        }





        [HttpPost("SetSerialPort")]
        public IActionResult SetSerialPort(string portName, int stopBits, int baudRate, int dataBits, int parity, int readTimeout, int writeTimeout)
        {
            #region 检查检查进程是否存在
            var res = _dcDbContext.processInfos.Where(u => true);
            if (res != null)
            {
                Process[] pro = Process.GetProcesses();//获取bai已开启du的所有进程
                var data = from i in res join j in pro on i.processId equals j.Id select j;
                if (data != null)
                {
                    return new JsonResult(new { serverData = "no", returnCode = ReturnCode.ServerError });
                }
            }
            #endregion
            _continue = true;
            #region MyRegion
            //PortName = "COM3",
            //StopBits = StopBits.One,         
            //BaudRate = 115200,
            //DataBits = 8,
            //Parity = Parity.None,
            //ReadTimeout = 500,
            //WriteTimeout = 500 
            #endregion
            try
            {
                _mySerialPort.guid = System.Guid.NewGuid().ToString();
                _mySerialPort.PortName = portName;
                _mySerialPort.StopBits = (StopBits)stopBits;
                _mySerialPort.BaudRate = baudRate;
                _mySerialPort.DataBits = dataBits;
                _mySerialPort.Parity = (Parity)parity;
                _mySerialPort.ReadTimeout = readTimeout;
                _mySerialPort.WriteTimeout = writeTimeout;
                string str = AppDomain.CurrentDomain.BaseDirectory;
                string excuteFile = Path.GetFullPath("..") + @"\Misc.DC.NULL\bin\Release\net5.0\publish\Misc.DC.NULL.exe";
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(portName);
                stringBuilder.Append(":");
                stringBuilder.Append(stopBits);
                stringBuilder.Append(":");
                stringBuilder.Append(baudRate);
                stringBuilder.Append(":");
                stringBuilder.Append(dataBits);
                stringBuilder.Append(":");
                stringBuilder.Append(parity);
                stringBuilder.Append(":");
                stringBuilder.Append(readTimeout);
                stringBuilder.Append(":");
                stringBuilder.Append(writeTimeout);
                string excuteFilePara = stringBuilder.ToString();
                Console.WriteLine(str);
                Process process = Process.Start(excuteFile, excuteFilePara);                            //启动一个数据进程
                ProcessInfo processInfo = new ProcessInfo()
                {
                    processId = process.Id,
                    processName = process.ProcessName
                };
                _dcDbContext.processInfos.Add(processInfo);
                _dcDbContext.SaveChanges();

                //SerialPortDataServer dataServer = new SerialPortDataServer(_continue, _mySerialPort);
                //_serialPortDataServer.ConfigSerial(_continue, _mySerialPort);
                //_serialPortDataServer.LoadData();
            }
            catch (Exception ex)

            {
                return new JsonResult(new { serverData = ex, returnCode = ReturnCode.ServerError });
            }
            return new JsonResult(new { serverData = "true", returnCode = ReturnCode.ServerOK });
        }
        [HttpGet("GetSerialPortName")]

        public IActionResult GetSerialPortName()
        {
            var res = SerialPort.GetPortNames();
            return new JsonResult(new { serverData = res, returnCode = ReturnCode.ServerOK });
        }
        [HttpGet("GetSerialPort")]
        public IActionResult GetSerialPort()
        {
            return new JsonResult(new { serverData = _mySerialPort, returnCode = ReturnCode.ServerOK });

        }
        [HttpGet("StopSerialPort")]
        public IActionResult StopSerialPort()
        {
            _continue = false;
            return new JsonResult(new { serverData = "true", returnCode = ReturnCode.ServerOK }); ;

        }

        [HttpGet("StartProcess")]
        public IActionResult StartProcess()
        {
            Process process = Process.Start(@"C:\Program Files\Internet Explorer\IExplore.exe", "www.northwindtraders.com");
            ProcessInfo processInfo = new ProcessInfo()
            {
                processId = process.Id,
                processName = process.ProcessName
            };
            _dcDbContext.processInfos.Add(processInfo);
            _dcDbContext.SaveChanges();

            return new JsonResult(new { serverData = process.Id, returnCode = ReturnCode.ServerOK }); ;
        }


        [HttpGet("KillProcess")]
        public IActionResult KillProcess(int id)
        {
            Process[] pro = Process.GetProcesses();//获取bai已开启du的所有进程
                                                   //遍历所有查找到zhi的进程
            for (int i = 0; i < pro.Length; i++)
            {
                //判断此进程是dao否是要查找的进程
                if (pro[i].Id == id)
                {
                    pro[i].Kill();//结束进程
                }
            }
            return new JsonResult(new { serverData = "ok", returnCode = ReturnCode.ServerOK }); ;
        }



    }
}
