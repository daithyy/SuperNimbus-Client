using UnityEngine;

public class SendController : MonoBehaviour
{
    private static void SendTcpData(Packet packet)
    {
        packet.WriteLength();

        Client.Instance.Tcp.SendData(packet);
    }

    private static void SendUdpData(Packet packet)
    {
        packet.WriteLength();

        Client.Instance.Udp.SendData(packet);
    }

    #region Packets

    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.WelcomeReceived))
        {
            packet.Write(Client.Instance.ClientId);
            packet.Write(GameManager.Instance.User.Username);

            SendTcpData(packet);
        }
    }

    public static void PlayerMovement(Vector3 inputDirection, bool[] actions)
    {
        using (Packet packet = new Packet((int)ClientPackets.PlayerMovement))
        {
            PlayerManager playerManager = EntityManager.Players[Client.Instance.ClientId];

            packet.Write(inputDirection);

            packet.Write(playerManager.transform.rotation);

            packet.Write(playerManager.transform.GetChild(0).transform.eulerAngles);

            packet.Write(actions.Length);

            for (int i = 0; i < actions.Length; i++)
            {
                packet.Write(actions[i]);
            }

            SendUdpData(packet);
        }
    }

    public static void MessageClient(string message)
    {
        using (Packet packet = new Packet((int)ClientPackets.MessageClient))
        {
            packet.Write(message);

            SendTcpData(packet);
        }
    }

    #endregion
}
