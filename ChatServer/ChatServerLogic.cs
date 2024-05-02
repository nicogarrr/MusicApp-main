using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatServer.Net.IO;

namespace ChatServer
{
    // This class is responsible for managing the users and their connections.
    // It will broadcast messages, connections, and disconnections to all users.
    // It will also handle the addition of new users.
    // This code is for Business Logic.
    public class ChatServerLogic
    {
        private static List<Client> users;

        public static void InitializeUsersList()
        {
            users = new List<Client>();
        }

        public static void AddUser(Client user)
        {
            users.Add(user);
            BroadcastConnection();
        }

        public static void BroadcastConnection()
        {
            foreach (var user in users)
            {
                foreach (var usr in users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastMessage(string message)
        {
            foreach (var user in users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(2);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            users.Remove(disconnectedUser);

            foreach (var user in users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(3);
                broadcastPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }

            BroadcastMessage($"[{disconnectedUser.Username}] Connection Error!");
        }
    }
}
