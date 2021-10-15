using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Dictionary<int, PlayerManager> Players = new Dictionary<int, PlayerManager>();

    public static Dictionary<int, Spawner> Spawners = new Dictionary<int, Spawner>();

    public GameObject LocalPlayerPrefab;

    public GameObject PlayerPrefab;

    public GameObject SpawnerPrefab;

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

    public void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject player; 

        if (id == Client.Instance.MyId)
        {
            player = Instantiate(LocalPlayerPrefab, position, rotation);
            EventManager.RaiseOnRetrieveLocalPlayerInfo(id, username);
        }
        else
        {
            player = Instantiate(PlayerPrefab, position, rotation);
        }

        player.GetComponent<PlayerManager>().Id = id;
        player.GetComponent<PlayerManager>().Username = username;
        player.GetComponent<PlayerManager>().JumpController = player.GetComponent<JumpController>();

        Players.Add(id, player.GetComponent<PlayerManager>());
    }

    public void CreateSpawner(int id, Vector3 position, bool hasItem)
    {
        GameObject spawner = Instantiate(SpawnerPrefab, position, SpawnerPrefab.transform.rotation);

        Spawner instance = spawner.GetComponent<Spawner>();

        instance.Initialize(id, hasItem);

        Spawners.Add(id, instance);
    }
}
