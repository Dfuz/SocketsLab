﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace labSockets
{
    internal class Server : ISocketsLab
    {
        private readonly int _port; // порт для приема входящих запросов
        private static readonly int NumberOfConnections = 4; // максимальное число одновременных подключений
        private const string m_fileName = @"log.txt"; // файл для логов
        private TextWriter m_fileWriter = null;
        private List<Thread> connectionList;
        public Server(int port)
        {
            _port = port;
            connectionList = new List<Thread>();
        }

        public void Start()
        {
            try
            {
                var hostInfo = Dns.GetHostEntry("localhost");
                var listener = new TcpListener(hostInfo.AddressList[1], _port);

                listener.Start(1);

                m_fileWriter = TextWriter.Synchronized(new StreamWriter(m_fileName, true));
                Console.WriteLine("Сервер запущен по адресу: " + hostInfo.AddressList[1] +  ":" + _port);
                Console.WriteLine("Ожидание подключений...");
                while (true)
                {
                    if (connectionList.Count < NumberOfConnections)
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        ClientObject clientObject = new ClientObject(client, m_fileWriter);
                        Console.WriteLine("Подключен клиент с IP: " + Convert.ToString(((IPEndPoint)client.Client.RemoteEndPoint).Address) + ":"
                                                                    + Convert.ToString(((IPEndPoint)client.Client.RemoteEndPoint).Port));

                        // создаем новый поток для обслуживания нового клиента
                        var clientThread = new Thread(new ThreadStart(clientObject.Process));
                        clientThread.Start();
                        connectionList.Add(clientThread);
                    }
                    connectionList.RemoveAll(thr => thr.IsAlive == false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
