using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void OnConnectToServer(bool isConnected);

    public static event OnConnectToServer onServerConnect;

    public static void RaiseOnServerConnect(bool isConnected)
    {
        if (onServerConnect != null)
        {
            onServerConnect(isConnected);
        }
    }

    public delegate void OnRetrievePlayerInfo(int id, string username);

    public static event OnRetrievePlayerInfo onRetreivePlayerInfo;

    public static void RaiseOnRetrieveLocalPlayerInfo(int id, string username)
    {
        if (onRetreivePlayerInfo != null)
        {
            onRetreivePlayerInfo(id, username);
        }
    }
}
