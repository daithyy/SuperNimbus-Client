using System.Net;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int myId = packet.ReadInt();

        Debug.Log($"SERVER MESSAGE: {msg}");

        Client.Instance.MyId = myId;

        ClientSend.WelcomeReceived();

        Client.Instance.Udp.Connect(((IPEndPoint)Client.Instance.Tcp.Socket.Client.LocalEndPoint).Port);
    }

    public static void UdpTest(Packet packet)
    {
        string msg = packet.ReadString();

        Debug.Log($"SERVER MESSAGE: Received packet via UDP: {msg}");

        ClientSend.UdpTestReceived();
    }
}
