using System.Net.Sockets;
using System;
using UnityEngine;

public partial class Client
{
    public class TCP
    {
        public TcpClient Socket;

        private NetworkStream stream;

        private Packet receivedData;

        private byte[] receiveBuffer;

        public void Connect()
        {
            Socket = new TcpClient()
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };

            receiveBuffer = new byte[DataBufferSize];

            Socket.BeginConnect(Instance.Ip, Instance.Port, ConnectCallback, Socket);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (Socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"ERROR: Sending data to server via TCP {ex}");
                UIManager.Instance.SendMessage(new Message(Constants.GameId, "<color=#FF0041>Lost connection to Game Server.</color>", DateTime.Now));
            }
        }

        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                Socket.EndConnect(result);
            }
            catch
            {
                EventManager.RaiseOnServerConnect(false);
            }

            if (!Socket.Connected)
            {
                return;
            }

            stream = Socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLength = stream.EndRead(result);

                if (byteLength <= 0)
                {
                    Instance.Disconnect();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                // Stream based protocol TCP
                // Reset data only when it is fully received
                receivedData.Reset(HandleData(data));

                stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Receiving TCP Data: {ex}");
                Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLength = 0;

            receivedData.SetBytes(data);

            if (receivedData.UnreadLength() >= 4)
            {
                packetLength = receivedData.ReadInt();

                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
            {
                byte[] packetBytes = receivedData.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();

                        packetHandlers[packetId](packet);
                    }
                });

                packetLength = 0;

                if (receivedData.UnreadLength() >= 4)
                {
                    packetLength = receivedData.ReadInt();

                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        private void Disconnect()
        {
            Instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            Socket = null;
        }
    }
}
