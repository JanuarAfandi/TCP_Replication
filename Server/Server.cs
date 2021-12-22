using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DataPemain;

namespace Server
{
    public class Constant
    {
        public static byte[] dataBuffer = new byte[255];
    }
    class Server
    {
        TcpListener listener;
        private string ip;
        private int port;

        private List<Client> clientOnServer = new List<Client>();
        public delegate void PacketHandler(int _fromClient);
        public static Dictionary<int, PacketHandler> packetHandlers;
        public Server(string _ip, int _port)
        {
            ip = _ip;
            port = _port;
        }

        public void Start()
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            listener.Start();
            Console.WriteLine("Server Dimulai");
            listener.BeginAcceptTcpClient(ConnectionCallback, null);
        }

        private void ConnectionCallback(IAsyncResult _result)
        {
            TcpClient client = listener.EndAcceptTcpClient(_result);
            listener.BeginAcceptTcpClient(ConnectionCallback, null);

            // add player on server
            Client newPlayer = new Client(client);
            clientOnServer.Add(newPlayer);
            Console.WriteLine($"Koneksi masuk dari {client.Client.RemoteEndPoint}...");
        }

    }
    public class Client
    {
        private TcpClient socket;
        private NetworkStream stream;
        private Player player;

        public Client(TcpClient _client)
        {
            socket = _client;
            socket.ReceiveBufferSize = Constant.dataBuffer.Length;
            socket.SendBufferSize = Constant.dataBuffer.Length;

            stream = socket.GetStream();

            stream.BeginRead(Constant.dataBuffer, 0, Constant.dataBuffer.Length, ReceiveData, null);
        }

        private void ReceiveData(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    // disconnected
                    return;
                }

                byte[] data = new byte[_byteLength];
                Array.Copy(Constant.dataBuffer, data, _byteLength);

                HandleData(data);
                stream.BeginRead(Constant.dataBuffer, 0, Constant.dataBuffer.Length, ReceiveData, null);
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error menerima data TCP: {_ex}");
                // disconnected
            }
        }
        private void HandleData(byte[] data)
        {
            byte[] buffer = data;
            int readPos = 0;

            while (buffer.Length - readPos >= 4)
            {
                int packetType = BitConverter.ToInt32(buffer, readPos);
                readPos += 4;

                switch (packetType)
                {
                    case (int)Packet.SpawnPemain:
                        player = new Player(0, 0);
                        break;
                    case (int)Packet.MovePemain:
                        int x = BitConverter.ToInt32(buffer, readPos);
                        readPos += 4;
                        int y = BitConverter.ToInt32(buffer, readPos);
                        readPos += 4;
                        player.Move(x, y);
                        break;
                    case (int)Packet.AttackPemain:
                        player.Attack();
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
