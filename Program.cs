using System;
using System.Net;

namespace NamingServer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.Write("IP to start naming server on: ");
            string address = Console.ReadLine();
            IPAddress ipAddress;
            if (IPAddress.TryParse(address, out ipAddress))
            {
                NameServer server = new NameServer(ipAddress);
                server.StartServer();
            }
            else
            {
                Console.WriteLine("Not a valid IP address.");
            }
            Console.ReadLine();
        }
    }
}
