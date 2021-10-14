using System;
using System.Net;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int myId = packet.ReadInt();

        Debug.Log($"SERVER: {msg}");
        UIManager.Instance.ReceiveMessage(new Message(Constants.ServerId, msg, DateTime.Now));

        Client.Instance.MyId = myId;

        SendController.WelcomeReceived();

        Client.Instance.Udp.Connect(((IPEndPoint)Client.Instance.Tcp.Socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.Instance.SpawnPlayer(id, username, position, rotation);
    }

    public static void PlayerPosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        if (GameManager.Players.TryGetValue(id, out PlayerManager player))
        {
            player.transform.position = position;
        }
    }

    public static void PlayerRotation(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();
        Vector3 eulerAngles = packet.ReadVector3();

        if (GameManager.Players.TryGetValue(id, out PlayerManager player))
        {
            player.transform.rotation = rotation;
            player.transform.GetChild(0).eulerAngles = eulerAngles;
        }
    }

    public static void PlayerAnimation(Packet packet)
    {
        int id = packet.ReadInt();
        bool jumping = packet.ReadBool();
        bool grounded = packet.ReadBool();

        GameManager.Players[id].JumpController.ReadActions(jumping, grounded);
    }

    public static void PlayerDisconnected(Packet packet)
    {
        int id = packet.ReadInt();

        Destroy(GameManager.Players[id].gameObject);
        GameManager.Players.Remove(id);
    }

    public static void CreateSpawner(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        bool hasItem = packet.ReadBool();

        GameManager.Instance.CreateSpawner(id, position, hasItem);
    }

    public static void ItemSpawn(Packet packet)
    {
        int id = packet.ReadInt();

        GameManager.Spawners[id].ItemSpawn();
    }

    public static void ItemCollect(Packet packet)
    {
        int id = packet.ReadInt();
        int playerId = packet.ReadInt();

        GameManager.Spawners[id].ItemCollect();
        GameManager.Players[playerId].ItemCount++;
    }

    public static void MessageServer(Packet packet)
    {
        int id = packet.ReadInt();
        string message = packet.ReadString();
        DateTime datetime = DateTime.TryParse(packet.ReadString(), out datetime) ? datetime : new DateTime();
        Message chatMsg = new Message(id, message, datetime);

        UIManager.Instance.ReceiveMessage(chatMsg);
    }
}
