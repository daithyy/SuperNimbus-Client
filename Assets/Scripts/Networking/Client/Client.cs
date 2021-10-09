using System.Collections.Generic;
using UnityEngine;

public partial class Client : MonoBehaviour
{
    private const int BufferConstant = 4096;

    private static Dictionary<int, PacketHandler> packetHandlers;

    private delegate void PacketHandler(Packet packet);

    public static Client Instance;

    public static readonly int DataBufferSize = BufferConstant;

    public string Ip = "127.0.0.1";

    public int Port = 26950;

    public int MyId = 0;

    public TCP Tcp;

    public UDP Udp;

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

    private void Start()
    {
        Tcp = new TCP();
        Udp = new UDP();
    }

    public void ConnectToServer()
    {
        InitializeClientData();

        Tcp.Connect();
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.Welcome, ClientHandler.Welcome },
            { (int)ServerPackets.UdpTest, ClientHandler.UdpTest }
        };

        Debug.Log("Initialize packets");
    }
}
