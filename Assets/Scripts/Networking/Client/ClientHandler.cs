using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int myId = packet.ReadInt();

        Debug.Log($"Message from server: {msg}");

        Client.Instance.MyId = myId;
    }
}
