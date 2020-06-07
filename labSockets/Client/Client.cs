using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace labSockets
{
    internal class Client : ISocketsLab
    {
        // адрес и порт сервера, к которому будем подключаться
        private readonly int _port; // порт сервера
        private readonly string _address; // адрес сервера
        private const string WritePath = @"log_client.txt"; // лог-файл клиента
        private static TcpClient _client;
        public Client(int port, string address)
        {
            _port = port;
            _address = address;

            // Establish an event handler to process key press events.
            Console.CancelKeyPress += MyHandler;
        }
        public void Start()
        {
            try
            {
                _client = new TcpClient(_address, _port);
                var stream = _client.GetStream();
                while (true)
                {
                    var message = Console.ReadLine();

                    // преобразуем сообщение в массив байтов
                    byte[] data = Encoding.UTF8.GetBytes(message ?? string.Empty);

                    // отправка сообщения
                    stream.Write(data, 0, data.Length);

                    var myIpClient = Convert.ToString(((IPEndPoint)_client.Client.RemoteEndPoint).Address);
                    var myPortClient = Convert.ToString(((IPEndPoint)_client.Client.RemoteEndPoint).Port);

                    using (StreamWriter sw = new StreamWriter(WritePath, true, Encoding.Default))
                    {
                        sw.WriteLine("Date: " + DateTime.Now.ToString() + " IP: " + myIpClient + ":" + myPortClient + " Message: " + message);
                    }

                    // получаем ответ
                    data = new byte[256]; // буфер для ответа
                    var builder = new StringBuilder();

                    do
                    {
                        var bytes = stream.Read(data, 0, data.Length); // количество полученных байт
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    message = builder.ToString();
                    Console.WriteLine("Ответ сервера: " + message);

                    using (var sw = new StreamWriter(WritePath, true, Encoding.Default))
                    {
                        sw.WriteLine("Date: " + DateTime.Now.ToString() + " IP: " + myIpClient + ":" + myPortClient + " Response: " + message);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _client?.Close();
                Console.ReadKey(true);
            }
        }

        private static void MyHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Выход из программы...");
            _client?.Close();
        }
    }
}
