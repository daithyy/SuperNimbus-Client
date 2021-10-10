
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public partial class Client
{
    public class UDP
    {
        public UdpClient Socket;
        public IPEndPoint EndPoint;

        public UDP() => EndPoint = new IPEndPoint(IPAddress.Parse(Instance.Ip), Instance.Port);

        public void Connect(int localPort)
        {
            Socket = new UdpClient(localPort);

            Socket.Connect(EndPoint);
            Socket.BeginReceive(ReceiveCallback, null);

            using (Packet packet = new Packet())
            {
                SendData(packet);
            }
        }

        public void SendData(Packet packet)
        {
            try
            {
                packet.InsertInt(Instance.MyId);

                if (Socket != null)
                {
                    Socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                }
            }
            catch (Exception ex) 
            {
                Debug.Log($"ERROR: Sending data to server via UDP: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult actionResult)
        {
            try
            {
                byte[] data = Socket.EndReceive(actionResult, ref EndPoint);
                Socket.BeginReceive(ReceiveCallback, null);

                if (data.Length < 4)
                {
                    Instance.Disconnect();
                    return;
                }

                HandleData(data);
            }
            catch (Exception ex)
            {
                Disconnect();
            }
        }

        private void HandleData(byte[] data)
        {
            using (Packet packet = new Packet(data))
            {
                int packetLength = packet.ReadInt();

                data = packet.ReadBytes(packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(data))
                {
                    int packetId = packet.ReadInt();

                    packetHandlers[packetId](packet);
                }
            });
        }

        private void Disconnect()
        {
            Instance.Disconnect();

            EndPoint = null;
            Socket = null;
        }
    }
}
