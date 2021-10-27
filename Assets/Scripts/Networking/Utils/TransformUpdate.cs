using UnityEngine;

public class TransformUpdate
{
    public int Tick;

    public Vector3 Position;

    public Quaternion Rotation;

    public TransformUpdate(int tick, Vector3 position)
    {
        Tick = tick;
        Position = position;
        Rotation = Quaternion.identity;
    }

    public TransformUpdate(int tick, Quaternion rotation)
    {
        Tick = tick;
        Position = Vector3.zero;
        Rotation = rotation;
    }

    public TransformUpdate(int tick, Vector3 position, Quaternion rotation)
    {
        Tick = tick;
        Position = position;
        Rotation = rotation;
    }

    public TransformUpdate(int tick, Transform transform, bool isLocalRotation = false)
    {
        Tick = tick;
        Position = transform.position;

        if (isLocalRotation)
        {
            Rotation = transform.localRotation;
        }
        else
        {
            Rotation = transform.rotation;
        }
    }
}