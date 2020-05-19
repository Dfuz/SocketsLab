using System;
using System.Net.Sockets;

namespace labSockets
{
    class Program
    {
        private string port;
        private string adress;
        static void Main(string[] args)
        {
            var startParam = '0';
            while (startParam != '1' && startParam != '2')
            {
                Console.Write("1 - to start server, 2 - client $ ");
                startParam = Console.ReadKey().KeyChar;
                Console.WriteLine();
            }
            Console.Write("1 - to start server, 2 - client $ ");
        }
    }
}
