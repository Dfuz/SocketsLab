using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace labSockets
{
    internal class Server : ISocketsLab
    {
        private readonly int port; // порт для приема входящих запросов
        private static readonly int numberOfConnections = 5; // максимальное число одновременных подключений
        private const string m_fileName = @"log.txt"; // файл для логов
        private TextWriter m_fileWriter = null;
        List<Thread> connectionList;
        public Server(int _port)
        {
            port = _port;
            connectionList = new List<Thread>();
        }
        private static ConcurrentQueue<string> stringQueue = new ConcurrentQueue<string>();
        private void TaskWriter()
        {
            //var source = new CancellationTokenSource();
            //var token = source.Token;
            //Task.Run(async () => {
            //    using (var stream = File.OpenWrite(writePath))
            //    using (var streamWriter = new StreamWriter(stream))
            //    {
            //        while (true)
            //        {
            //            if (token.IsCancellationRequested)
            //            {
            //                streamWriter.Flush();
            //                return;
            //            }

            //            string kal;
            //            while (stringQueue.TryDequeue(out kal))
            //            {
            //                streamWriter.WriteLine(kal);
            //            }
            //            // No data, let's delay
            //            await Task.Delay(500);
            //        }
            //    }
            //}, token);
            m_fileWriter = TextWriter.Synchronized(new StreamWriter(m_fileName, true));
        }
        public void Start()
        {
            try
            {
                var hostInfo = Dns.GetHostEntry("localhost");
                //var listener = new TcpListener(hostInfo.AddressList[0], port);
                var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start(1);
                //TaskWriter();
                m_fileWriter = TextWriter.Synchronized(new StreamWriter(m_fileName, true));
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                while (true)
                {
                    if (connectionList.Count < numberOfConnections)
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        ClientObject clientObject = new ClientObject(client, m_fileWriter);

                        // создаем новый поток для обслуживания нового клиента
                        Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                        clientThread.Start();
                        
                        connectionList.Add(clientThread);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
