using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        public IConfiguration _configuration { get; set; }
        public ConfigureController(MySerialPort mySerialPort,/* SerialPortDataServer serialPortDataServer,*/ DcDbContext dcDbContext, IConfiguration configuration)
        {
            _continue = true;
            _mySerialPort = mySerialPort;
            //  _serialPortDataServer = serialPortDataServer;
            _dcDbContext = dcDbContext;
            _configuration = configuration;
        }
        #region MyRegion
        //检查进程是否存在
        //[HttpGet("CheckProcessExist")]
        //public IActionResult CheckProcessExist()
        //{
        //    return null;
        //} 
        #endregion


        [HttpPost("SetSerialPort")]
        public IActionResult SetSerialPort([FromForm] string portName, [FromForm] int stopBits, [FromForm] int baudRate, [FromForm] int dataBits, [FromForm] int parity, [FromForm] int readTimeout, [FromForm] int writeTimeout)
        {
            #region 检查检查进程是否存在
            var res = _dcDbContext.processInfos.Where(u => true).ToList();
            if (res != null)
            {
                Process[] pro = Process.GetProcesses();//获取已开启的所有进程
                var data = from i in res join j in pro on i.processId equals j.Id where i.processName == j.ProcessName select j;
                if (data.ToList().Count > 0)
                {
                    return new JsonResult(new { sInfo = "检测到进程已经在运行,请停止后再尝试.", returnCode = ReturnCode.ServerError });
                    //   return new JsonResult(new { serverData = "no", returnCode = ReturnCode.ProcessExisted });
                }
            }
            #endregion
            _continue = true;
            string sInfo = "";
            //    ProcessInfo info = null;
            string excuteFilePath = _configuration.GetSection("ServerConfig:Path").Value;
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
                //  string str = AppDomain.CurrentDomain.BaseDirectory;
                //string excuteFile = Path.GetFullPath("..") + path;
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
                // Console.WriteLine(str);
                //检查进程是否存在（数据库中和系统中）
                //info = _dcDbContext.processInfos.Where(u => true).FirstOrDefault();
                //if (info != null)
                //{
                //    Process[] pro = Process.GetProcesses();//获取已开启的所有进程
                //                                           //遍历所有查找到zhi的进程
                //    for (int i = 0; i < pro.Length; i++)
                //    {

                //        if (pro[i].Id == info.processId)
                //        {
                //            return new JsonResult(new { serverInfo = sInfo, serverData = "检测到进程已经在运行,请停止后再尝试.", returnCode = ReturnCode.ServerError });
                //        }
                //    }
                //}
                Process process = Process.Start(excuteFilePath, excuteFilePara);    //启动一个数据进程
                                                                                    //  sInfo = process.StandardOutput.ReadToEnd();                         //读取process的结果
                ProcessInfo processInfo = new ProcessInfo()
                {
                    processId = process.Id,
                    processName = process.ProcessName,
                    comName = portName,
                };
                _dcDbContext.processInfos.Add(processInfo);
                _dcDbContext.SaveChanges();
                //SerialPortDataServer dataServer = new SerialPortDataServer(_continue, _mySerialPort);
                //_serialPortDataServer.ConfigSerial(_continue, _mySerialPort);
                //_serialPortDataServer.LoadData();
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    sInfo = ex.Message,
                    returnCode = ReturnCode.ServerError
                });
            }
            return new JsonResult(new { returnCode = ReturnCode.ServerOK, sInfo = "服务启动成功" });
        }


        [HttpGet("GetAllSerialPortName")]
        public IActionResult GetAllSerialPortName()
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

        //获取已经打开的串口
        [HttpGet("GetOpendSerialPort")]
        public IActionResult GetOpendSerialPort()
        {
            var res = _dcDbContext.processInfos.Where(u => true);
            return new JsonResult(new { serverData = _mySerialPort, returnCode = ReturnCode.ServerOK });
        }

        //获取可用的串口
        [HttpGet("GetAvailableSerialPort")]
        public IActionResult GetAvailableSerialPort()
        {
            var res0 = SerialPort.GetPortNames();
            var res1 = _dcDbContext.processInfos.Where(u => true);
            return new JsonResult(new { serverData = _mySerialPort, returnCode = ReturnCode.ServerOK });
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
            return new JsonResult(new { serverData = process.Id, returnCode = ReturnCode.ServerOK });
        }

        [HttpGet("KillProcess")]
        public IActionResult KillProcess(int id)
        {
            Process[] pro = Process.GetProcesses();             //获取已开启的所有进程
            var res = _dcDbContext.processInfos.Where(u => u.id == id).FirstOrDefault();
            for (int i = 0; i < pro.Length; i++)                //遍历所有查找到的进程
            {
                if (pro[i].Id == res.processId)  //判断此进程是否是要查找的进程
                {
                    pro[i].Kill();//结束进程
                }
            }
            _dcDbContext.processInfos.Remove(res);      //需要从数据库移除进程id
            var data = _dcDbContext.SaveChanges() > 0 ? true : false;
            return new JsonResult(new { serverData = "ok", returnCode = ReturnCode.ServerOK, data });
        }

        [HttpGet("GetRunningProcess")]
        public IActionResult GetRunningProcess()
        {
            var res = _dcDbContext.processInfos.Where(u => true).FirstOrDefault();
            return new JsonResult(new { serverData = res, returnCode = ReturnCode.ServerOK });
        }

        [HttpGet("ClearCache")]
        public IActionResult ClearCache()
        {
            bool flag = false;
            var res = _dcDbContext.processInfos.Where(u => true);
            _dcDbContext.processInfos.RemoveRange(res);
            bool data = _dcDbContext.SaveChanges() > 0 ? true : false;
            Process[] pro = Process.GetProcesses();
            for (int i = 0; i < pro.Length; i++)
            {
                if (pro[i].ProcessName == "Misc.DC.NULL")  //判断此进程是否是要查找的进程
                {
                    pro[i].Kill();//结束进程
                }
            }
            flag = true;
            if (data || flag)
            {
                return new JsonResult(new { sInfo = "刷新成功", returnCode = ReturnCode.ServerOK });
            }
            else
            {
                return new JsonResult(new { sInfo = "刷新失败", returnCode = ReturnCode.ServerError });
            }
        }
    }
}
