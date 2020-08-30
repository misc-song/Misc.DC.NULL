using Misc.DC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Misc.DC.DataSource
{
    public class SocketServer
    {
        public IPAddress _ip { get; set; }
        public bool _continue { get; set; }
        public int _port { get; set; }
        public ConcurrentQueue<TempAndHumid> _tempAndHumids { get; set; }
        public SocketServer(bool __continue, IPAddress ip, int port, ConcurrentQueue<TempAndHumid> tempAndHumids)
        {
            _continue = __continue;
            _ip = ip;
            _port = port;
            _tempAndHumids = tempAndHumids;
        }
        public Dictionary<string, Socket> dic = new Dictionary<string, Socket>();
        public void Start()
        {
            IPEndPoint point = new IPEndPoint(_ip, _port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Bind(point);
                socket.Listen(10);
                Console.WriteLine("服务器开始监听");
                Thread thread = new Thread(AcceptInfo);
                thread.IsBackground = true;
                thread.Start(socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //监听连接
        public void AcceptInfo(object o)
        {
            Socket socket = o as Socket;
            while (_continue)
            {
                try
                {
                    Socket tSocket = socket.Accept();
                    string point = tSocket.RemoteEndPoint.ToString();
                    Console.WriteLine(point + "连接成功！");
                    dic.Add(point, tSocket);
                    Thread thr = new Thread(ReceiveMsg);
                    //  th.IsBackground = true;
                    thr.Start(tSocket);
                    //thr.Join();
                    Thread thw = new Thread(SentMsg);
                    thw.Start();
                    //thw.Join();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }

        //接收信息
        public void ReceiveMsg(object o)
        {
            Socket client = o as Socket;
            while (_continue)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024];
                    int n = client.Receive(buffer);
                    string words = Encoding.UTF8.GetString(buffer, 0, n);
                    //Console.WriteLine(client.RemoteEndPoint.ToString() + ":" + words);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }
        //发送信息
        public void SentMsg()
        {
            try
            {
                //List<TempAndHumid> temps = new List<TempAndHumid>();
                //foreach (var i in _tempAndHumids)
                //{
                //    TempAndHumid tempAndHumid = new TempAndHumid();
                //    _tempAndHumids.TryDequeue(out tempAndHumid);
                //    temps.Append(tempAndHumid);
                //}
                while (_continue)
                {
                    while (!_tempAndHumids.IsEmpty)
                    {
                        TempAndHumid tempAndHumid = new TempAndHumid();
                        _tempAndHumids.TryDequeue(out tempAndHumid);
                        var msg = JsonConvert.SerializeObject(tempAndHumid);
                        byte[] buffer = Encoding.UTF8.GetBytes(msg);

                        //像所有活动的链接中发送数据
                        foreach (var res in dic)
                        {
                            dic[res.Key].Send(buffer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void StopServer()
        {
            _continue = false;
        }

    }
}