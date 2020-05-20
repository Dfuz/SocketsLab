using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace labSockets
{
    internal class ClientObject
    {
        private TcpClient client;
        private TextWriter m_fileWriter = null;
        public ClientObject(TcpClient tcpClient, TextWriter writer)
        {
            client = tcpClient;
            m_fileWriter = writer;
        }
        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();

                var data = new byte[256]; // буфер для получаемых данных
                while (true)
                {
                    // получаем сообщение
                    var builder = new StringBuilder();
                    var bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    } while (stream.DataAvailable);

                    var message = builder.ToString();

                    if (string.Empty == message || !client.Connected)
                    {
                        Console.WriteLine("Клиент отключился");
                        break;
                    }

                    string MyIpClient, MyPortClient;
                    MyIpClient = Convert.ToString(((IPEndPoint) client.Client.RemoteEndPoint).Address);
                    MyPortClient = Convert.ToString(((IPEndPoint) client.Client.RemoteEndPoint).Port);

                    Console.WriteLine("Date: " + DateTime.Now + " IP: " + MyIpClient + ":" + MyPortClient +
                                      " Message: " + message);

                    m_fileWriter.WriteLine("Date: " + DateTime.Now + " IP: " + MyIpClient + ":" + MyPortClient +
                                               " Message: " + message);
                    m_fileWriter.Flush();
                    // отправляем обратно сообщение в верхнем регистре

                    var response = new string(message.Reverse().ToArray());
                    //response += "\nСервер создал Перфильев В.Д.";
                    data = Encoding.UTF8.GetBytes(response);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
}