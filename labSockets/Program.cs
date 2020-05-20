using System;
using System.Net.Sockets;

namespace labSockets
{
    class Program
    {
        private static ISocketsLab App;
        private static string port;
        private static string address;
        
        static void Main(string[] args)
        {
            var startParam = '0';
            while (startParam != '1' && startParam != '2')
            {
                Console.Write("1 - to start server, 2 - client $ ");
                startParam = Console.ReadKey().KeyChar;
                Console.WriteLine();
            }
            Console.Write("Select port $ ");
            port = Console.ReadLine();
            if (startParam == '2')
            {
                Console.Write("Select address $ ");
                address = Console.ReadLine();
                App = new Client(Int32.Parse(port), address);
            }
            else
            {
                App = new Server(Int32.Parse(port));
            }
            App.Start();
        }
    }
}
