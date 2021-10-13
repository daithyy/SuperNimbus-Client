using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int Id;

    public bool HasItem;

    public MeshRenderer Model;

    public float ItemRotationSpeed = 50f;

    public float ItemBounceSpeed = 2f;

    private Vector3 position;

    public void Initialize(int id, bool hasItem)
    {
        Id = id;
        HasItem = hasItem;
        Model.enabled = hasItem;

        position = transform.position;
    }

    private void Update()
    {
        if (HasItem)
        {
            transform.Rotate(Vector3.up, ItemRotationSpeed * Time.deltaTime, Space.World);
            transform.position = position + new Vector3(0, 0.25f * Mathf.Sin(Time.time * ItemBounceSpeed), 0);
        }
    }

    public void ItemSpawn()
    {
        HasItem = true;
        Model.enabled = true;
    }

    public void ItemCollect()
    {
        HasItem = false;
        Model.enabled = false;
    }
}
