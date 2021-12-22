using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DataPemain;

namespace Client
{
    class Client
    {
        TcpClient socket;
        NetworkStream stream;
        string ip;
        int port;

        public Client(string ip, int port)
        {
            try
            {
                socket = new TcpClient(ip, port);
                stream = socket.GetStream();

                Console.WriteLine("Menyambung to server...");

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }

        public void SpawnPlayer()
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes((int)Packet.SpawnPemain));

            // Send the message to the connected server. 
            stream.Write(data.ToArray(), 0, data.Count);
        }

        public void PlayerMovement(int _x, int _y)
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes((int)Packet.MovePemain));
            data.AddRange(BitConverter.GetBytes(_x));
            data.AddRange(BitConverter.GetBytes(_y));

            // Send the message to the connected server. 
            stream.Write(data.ToArray(), 0, data.Count);
        }

        public void PlayerAttack()
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes((int)Packet.AttackPemain));

            // Send the message to the connected server. 
            stream.Write(data.ToArray(), 0, data.Count);
        }
    }
}
