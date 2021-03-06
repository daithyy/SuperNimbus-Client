using System.Collections.Generic;
using UnityEngine;

public partial class Client : MonoBehaviour
{
    private static Dictionary<int, PacketHandler> packetHandlers;

    private delegate void PacketHandler(Packet packet);

    public static Client Instance;

    public static readonly int DataBufferSize = Constants.BufferConstant;

    public string Ip = Constants.IpDefault;

    public int Port = Constants.Port;

    public int ClientId = 0;

    public TCP Tcp;

    public UDP Udp;

    private bool isConnected;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer(string ipAddress, int port)
    {
        Ip = ipAddress;
        Port = port;

        Tcp = new TCP();
        Udp = new UDP();

        InitializeClientData();

        isConnected = true;

        Tcp.Connect();
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.Welcome, ClientHandler.Welcome },
            { (int)ServerPackets.SpawnPlayer, ClientHandler.SpawnPlayer },
            { (int)ServerPackets.PlayerPosition, ClientHandler.PlayerPosition },
            { (int)ServerPackets.PlayerRotation, ClientHandler.PlayerRotation },
            { (int)ServerPackets.PlayerAnimation, ClientHandler.PlayerAnimation },
            { (int)ServerPackets.PlayerDisconnected, ClientHandler.PlayerDisconnected },
            { (int)ServerPackets.CreateSpawner, ClientHandler.CreateSpawner },
            { (int)ServerPackets.ItemSpawn, ClientHandler.ItemSpawn },
            { (int)ServerPackets.ItemCollect, ClientHandler.ItemCollect },
            { (int)ServerPackets.MessageServer, ClientHandler.MessageServer },
            { (int)ServerPackets.ServerValidate, ClientHandler.ServerValidate },
        };

        Debug.Log("Initialized packets");
    }

    private void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            
            Tcp.Socket.Close();
            Udp.Socket.Close();

            Debug.Log("Disconnected from Game Server.");
        }
    }
}
