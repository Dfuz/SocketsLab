﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace labSockets
{
    internal class Client : ISocketsLab
    {
        // адрес и порт сервера, к которому будем подключаться
        private readonly int port; // порт сервера
        private readonly string address; // адрес сервера
        public Client(int _port, string _address)
        {
            port = _port;
            address = _address;
        }
        public void Start()
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                Console.Write("Введите сообщение $ ");
                string message = Console.ReadLine();
                byte[] data = Encoding.UTF8.GetBytes(message);
                socket.Send(data);

                // получаем ответ
                data = new byte[256]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт

                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);
                Console.WriteLine("Ответ сервера: " + builder.ToString());

                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
