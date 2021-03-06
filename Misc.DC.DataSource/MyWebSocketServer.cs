﻿using Fleck;
using Microsoft.Extensions.Configuration;
using Misc.DC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Misc.DC.DataSource
{
    public class MyWebSocketServer
    {
        public bool _continue { get; set; }
        // public Dictionary<string, ConnectionInfo> dic = new Dictionary<string, WebSocketConnection>();
        IDictionary<string, IWebSocketConnection> dic_Sockets = new Dictionary<string, IWebSocketConnection>();
        public ConcurrentQueue<TempAndHumid> _tempAndHumids { get; set; }
        public string _address { get; set; }
        public string _port { get; set; }
        public MyWebSocketServer(ConcurrentQueue<TempAndHumid> tempAndHumids, string address, string port)
        {
            _continue = true;
            _tempAndHumids = tempAndHumids;
            _address = address;
            _port = port;
        }

        public void Start()
        {
            try
            {
                Console.WriteLine("服务器开始监听");
                Thread thread = new Thread(ConfigWebSocketServer);
                // thread.IsBackground = true;
                thread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "exception");
            }
        }

        public void ConfigWebSocketServer()
        {
            var server = new WebSocketServer("ws://" + _address + ":" + _port);
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    //连接成功 保存数据到字典
                    string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                    dic_Sockets.Add(clientUrl, socket);
                    //   dic.Add(socket.ConnectionInfo.ClientIpAddress, tSocket);
                    while (_continue)
                    {
                        while (!_tempAndHumids.IsEmpty)
                        {
                            TempAndHumid tempAndHumid = new TempAndHumid();
                            _tempAndHumids.TryDequeue(out tempAndHumid);
                            var msg = JsonConvert.SerializeObject(tempAndHumid);
                            if (msg == "null")
                            {
                                break;
                            }
                            byte[] buffer = Encoding.UTF8.GetBytes(msg);
                            //像所有活动的链接中发送数据
                            foreach (var item in dic_Sockets)
                            {
                                dic_Sockets[item.Key].Send(msg);
                            }
                            // dic[res.Key].Send(buffer);
                            // socket.Send(msg);
                        }
                    }
                    Console.WriteLine("连接成功");
                };
                socket.OnClose = () =>
                {
                    //关闭连接 从字典中移除数据
                    string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                    dic_Sockets.Remove(clientUrl);
                    Console.WriteLine("Close One Client");
                };
                socket.OnMessage = message =>  //接受客户端网页消息事件
                {
                    string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                    Console.WriteLine(DateTime.Now.ToString() + "|服务器:【收到】来客户端网页:" + clientUrl + "的信息：\n" + message);
                };
            });
        }
    }
}
