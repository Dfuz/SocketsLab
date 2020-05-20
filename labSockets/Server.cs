using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace labSockets
{
    internal class Server : ISocketsLab
    {
        private readonly int port; // порт для приема входящих запросов
        List<Thread> connectionList;
        public Server(int _port)
        {
            port = _port;
        }

        public void Start()
        {
            try
            {
                var hostInfo = Dns.GetHostEntry("localhost");

                // получаем адреса для запуска сокета
                var ipPoint = new IPEndPoint(hostInfo.AddressList[0], port);

                // создаем сокет
                var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(1);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    var handler = listenSocket.Accept();
                    // получаем сообщение
                    var builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

                    // отправляем ответ
                    string message = "ваше сообщение доставлено";
                    data = Encoding.Unicode.GetBytes(message);
                    handler.Send(data);
                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
