using System;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    internal class Program
    {
        private static TcpListener? listener;
        private static void Main(string[] args)
        {
            ChatServerLogic.InitializeUsersList();
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            listener.Start();

            while (true)
            {
                var client = new Client(listener.AcceptTcpClient());
                ChatServerLogic.AddUser(client);
            }
        }
    }
}
