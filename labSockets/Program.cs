using System;

namespace labSockets
{
    static class Program
    {
        private static ISocketsLab App;
        private static string port;
        private static string address;

        private static void Main(string[] args)
        {
            var startParam = '0';
            while (startParam != '1' && startParam != '2')
            {
                Console.Write("1 - запустить сервер, 2 - присоединиться как клиент $ ");
                startParam = Console.ReadKey().KeyChar;
                Console.WriteLine();
            }
            Console.Write("Выберите порт $ ");
            port = Console.ReadLine();
            if (startParam == '2')
            {
                Console.Write("Выберите адрес $ ");
                address = Console.ReadLine();
                App = new Client(int.Parse(port), address);
            }
            else
            {
                App = new Server(int.Parse(port));
            }
            App.Start();
        }
    }
}
