using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();

        Client.Instance.Tcp.SendData(packet);
    }

    private static void SendUDPData(Packet packet)
    {
        packet.WriteLength();

        Client.Instance.Udp.SendData(packet);
    }

    #region Packets

    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.WelcomeReceived))
        {
            packet.Write(Client.Instance.MyId);
            packet.Write(UIManager.Instance.UsernameField.text);

            SendTCPData(packet);
        }
    }

    public static void PlayerMovement(Vector3 inputDirection, bool[] actions)
    {
        using (Packet packet = new Packet((int)ClientPackets.PlayerMovement))
        {
            PlayerManager playerManager = GameManager.Players[Client.Instance.MyId];

            packet.Write(inputDirection);

            packet.Write(playerManager.transform.rotation);

            packet.Write(playerManager.transform.GetChild(0).transform.eulerAngles);

            packet.Write(actions.Length);

            for (int i = 0; i < actions.Length; i++)
            {
                packet.Write(actions[i]);
            }

            SendUDPData(packet);
        }
    }

    #endregion
}
