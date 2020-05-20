﻿using System;
using System.IO;
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
        const string writePath = @"log_client.txt";
        public Client(int _port, string _address)
        {
            port = _port;
            address = _address;
        }
        public void Start()
        {
            TcpClient client = null;
            try
            {
                client = new TcpClient(address, port);
                NetworkStream stream = client.GetStream();
                while (true)
                {
                    //Console.Write("Введите сообщение $ ");
                    string message = Console.ReadLine();

                    // преобразуем сообщение в массив байтов
                    byte[] data = Encoding.UTF8.GetBytes(message);

                    // отправка сообщения
                    stream.Write(data, 0, data.Length);

                    string MyIpClient, MyPortClient;
                    MyIpClient = Convert.ToString(((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address);
                    MyPortClient = Convert.ToString(((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Port);

                    using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLine("Date: " + DateTime.Now.ToString() + " IP: " + MyIpClient + ":" + MyPortClient + " Message: " + message);
                    }


                    // получаем ответ
                    data = new byte[256]; // буфер для ответа
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байт

                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    message = builder.ToString();
                    Console.WriteLine("Ответ сервера: " + message);

                    using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLine("Date: " + DateTime.Now.ToString() + " IP: " + MyIpClient + ":" + MyPortClient + " Response: " + message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (client != null) client.Close();
            }
            Console.ReadKey();
        }
    }
}
