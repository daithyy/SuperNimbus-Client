using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Dictionary<int, PlayerManager> Players = new Dictionary<int, PlayerManager>();

    public GameObject LocalPlayerPrefab;

    public GameObject PlayerPrefab;

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
        }
        else
        {
            player = Instantiate(PlayerPrefab, position, rotation);
        }

        player.GetComponent<PlayerManager>().Id = id;
        player.GetComponent<PlayerManager>().Username = username;

        Players.Add(id, player.GetComponent<PlayerManager>());
    }
}
