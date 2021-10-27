using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static Dictionary<int, PlayerManager> Players = new Dictionary<int, PlayerManager>();

    public static Dictionary<int, Spawner> Spawners = new Dictionary<int, Spawner>();

    public GameObject LocalPlayerPrefab;

    public GameObject PlayerPrefab;

    public GameObject ItemSpawnerPrefab;

    public void Player(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject prefab; 

        if (id == Client.Instance.ClientId)
        {
            prefab = Instantiate(LocalPlayerPrefab, position, rotation);
            EventManager.RaiseOnRetrieveLocalPlayerInfo(id, username);
        }
        else
        {
            prefab = Instantiate(PlayerPrefab, position, rotation);
        }

        PlayerManager playerManager = prefab.GetComponent<PlayerManager>();

        playerManager.Id = id;
        playerManager.Username = username;
        playerManager.JumpController = prefab.GetComponent<JumpController>();
        playerManager.Interpolator = prefab.GetComponent<PlayerInterpolator>();

        Players.Add(id, playerManager);
    }

    public void ItemSpawner(int id, Vector3 position, bool hasItem)
    {
        GameObject spawner = Instantiate(ItemSpawnerPrefab, position, ItemSpawnerPrefab.transform.rotation);

        Spawner instance = spawner.GetComponent<Spawner>();

        instance.Initialize(id, hasItem);

        Spawners.Add(id, instance);
    }
}
