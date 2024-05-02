﻿using System.Net.Sockets;
using System.Windows;
using MusicApp.Chat.Net.IO;

namespace MusicApp.Chat.Net
{
    internal class Server
    {
        private TcpClient client;
        public PacketReader PacketReader;

        public event Action ConnectedEvent;
        public event Action MsgReceivedEvent;
        public event Action DisconnectedEvent;

        public Server()
        {
            client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!client.Connected)
            {
                client.Connect("127.0.0.1", 7891);
                PacketReader = new PacketReader(client.GetStream());
                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteString(username);
                    client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    // TODO : differ between opcodes, by doing that function we could send files, images and all of that
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            ConnectedEvent?.Invoke();
                            break;
                        case 2:
                            MsgReceivedEvent?.Invoke();
                            break;
                        case 3:
                            DisconnectedEvent?.Invoke();
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        public void SendMessageToServer(string messsage)
        {
            try
            {
                var messagePacket = new PacketBuilder();
                messagePacket.WriteOpCode(2);
                messagePacket.WriteString(messsage);
                client.Client.Send(messagePacket.GetPacketBytes());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please log in before sending a message", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
