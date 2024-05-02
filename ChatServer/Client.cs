using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using ChatServer.Net.IO;

namespace ChatServer
{
    public class Client // We add public for seperating non-GUI classes
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        private readonly PacketReader packetReader;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            packetReader = new PacketReader(ClientSocket.GetStream());
            var opcode = packetReader.ReadByte();

            if (opcode == 0)
            {
                Username = packetReader.ReadMessage();
                Console.WriteLine($"[{DateTime.Now}]: Program connect with this username: {Username}");
            }
            else
            {
                Console.WriteLine("Connection Ended.");
                ClientSocket.Close();
            }
            Task.Run(() => Process());
        }

        private void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 2:
                            var msg = packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Mesage accepted! {msg}");
                            ChatServerLogic.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: {msg}");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"[{UID.ToString()}]: Connection Error!");
                    ChatServerLogic.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                }
            }
        }
    }
}
