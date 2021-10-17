using System;
using System.Net;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        EventManager.RaiseOnConnectToServer(true);

        string msg = packet.ReadString();
        int myId = packet.ReadInt();
        string datetime = packet.ReadString();

        Debug.Log($"SERVER: {msg}");
        GameManager.Instance.UI.SendMessage(new Message(Constants.ServerId, msg, datetime));

        Client.Instance.ClientId = myId;

        SendController.WelcomeReceived();

        Client.Instance.Udp.Connect(((IPEndPoint)Client.Instance.Tcp.Socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();
        string datetime = packet.ReadString();

        GameManager.Instance.Spawn.Player(id, username, position, rotation);
        GameManager.Instance.UI.SendMessage(new Message(Constants.ServerId, $"<color=#FFF545>{EntityManager.Players[id].Username}</color> has joined the game.", datetime));
    }

    public static void PlayerPosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        if (EntityManager.Players.TryGetValue(id, out PlayerManager player))
        {
            player.transform.position = position;
        }
    }

    public static void PlayerRotation(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();
        Vector3 eulerAngles = packet.ReadVector3();

        if (EntityManager.Players.TryGetValue(id, out PlayerManager player))
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

        EntityManager.Players[id].JumpController.ReadActions(jumping, grounded);
    }

    public static void PlayerDisconnected(Packet packet)
    {
        int id = packet.ReadInt();
        string datetime = packet.ReadString();

        GameManager.Instance.UI.SendMessage(new Message(Constants.ServerId, $"<color=#FFF545>{EntityManager.Players[id].Username}</color> has left the game.", datetime));

        Destroy(EntityManager.Players[id].gameObject);
        EntityManager.Players.Remove(id);
    }

    public static void CreateSpawner(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        bool hasItem = packet.ReadBool();

        GameManager.Instance.Spawn.ItemSpawner(id, position, hasItem);
    }

    public static void ItemSpawn(Packet packet)
    {
        int id = packet.ReadInt();

        EntityManager.Spawners[id].ItemSpawn();
    }

    public static void ItemCollect(Packet packet)
    {
        int id = packet.ReadInt();
        int playerId = packet.ReadInt();

        EntityManager.Spawners[id].ItemCollect();
        EntityManager.Players[playerId].ItemCount++;
    }

    public static void MessageServer(Packet packet)
    {
        int id = packet.ReadInt();
        string message = packet.ReadString();
        string datetime = packet.ReadString();
        Message chatMsg = new Message(id, message, datetime);

        GameManager.Instance.UI.SendMessage(chatMsg);
    }
}
