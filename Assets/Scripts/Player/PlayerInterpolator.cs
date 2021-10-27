using System.Collections.Generic;
using UnityEngine;

public class PlayerInterpolator : MonoBehaviour
{
    private enum InterpolatorMode
    {
        Both,
        Position,
        Rotation
    }

    [SerializeField] private InterpolatorMode mode = InterpolatorMode.Position;

    private List<TransformUpdate> futureTransformUpdates = new List<TransformUpdate>(); // Oldest first

    private TransformUpdate to;

    private TransformUpdate from;

    private TransformUpdate previous;

    [SerializeField] private float timeElapsed = 0f;

    [SerializeField] private float timeToReachTarget = 0.1f;

    [SerializeField] private bool isLocalRotation = true;

    [SerializeField] private float lerpSpeed = 3f;

    private void Start()
    {
        to = new TransformUpdate(GameManager.Instance.Tick, transform, isLocalRotation);
        from = new TransformUpdate(GameManager.Instance.TickDelay, transform, isLocalRotation);
        previous = new TransformUpdate(GameManager.Instance.TickDelay, transform, isLocalRotation);
    }

    private void Update()
    {
        for (int i = 0; i < futureTransformUpdates.Count; i++)
        {
            if (GameManager.Instance.Tick >= futureTransformUpdates[i].Tick)
            {
                previous = to;
                to = futureTransformUpdates[i];
                from = new TransformUpdate(GameManager.Instance.TickDelay, transform, isLocalRotation);
                futureTransformUpdates.RemoveAt(i);
                timeElapsed = 0;
                timeToReachTarget = (to.Tick - from.Tick) * Constants.SecondsPerTick;
            }
        }

        timeElapsed += Time.deltaTime;
        Interpolate((timeElapsed / timeToReachTarget) * lerpSpeed);
    }

    private void Interpolate(float lerpAmount)
    {
        switch (mode)
        {
            case InterpolatorMode.Both:
                if (isLocalRotation)
                {
                    InterpolatePosition(lerpAmount);
                    InterpolateLocalRotation(lerpAmount);
                }
                else
                {
                    InterpolatePosition(lerpAmount);
                    InterpolateRotation(lerpAmount);
                }
                break;
            case InterpolatorMode.Position:
                InterpolatePosition(lerpAmount);
                break;
            case InterpolatorMode.Rotation:
                if (isLocalRotation)
                {
                    InterpolateLocalRotation(lerpAmount);
                }
                else
                {
                    InterpolateRotation(lerpAmount);
                }
                break;
        }
    }

    private void InterpolatePosition(float lerpAmount)
    {
        if (to.Position == previous.Position)
        {
            // If this object isn't supposed to be moving, we don't want to interpolate and potentially extrapolate
            if (to.Position != from.Position)
            {
                // If this object hasn't reached it's intended position
                transform.position = Vector3.Lerp(from.Position, to.Position, lerpAmount); // Interpolate with the lerpAmount clamped so no extrapolation occurs
            }
            return;
        }

        transform.position = Vector3.LerpUnclamped(from.Position, to.Position, lerpAmount); // Interpolate with the lerpAmount unclamped so it can extrapolate
    }

    private void InterpolateRotation(float lerpAmount)
    {
        if (to.Rotation == previous.Rotation)
        {
            // If this object isn't supposed to be rotating, we don't want to interpolate and potentially extrapolate
            if (to.Rotation != from.Rotation)
            {
                // If this object hasn't reached it's intended rotation
                transform.rotation = Quaternion.Slerp(from.Rotation, to.Rotation, lerpAmount); // Interpolate with the lerpAmount clamped so no extrapolation occurs
            }
            return;
        }

        transform.rotation = Quaternion.SlerpUnclamped(from.Rotation, to.Rotation, lerpAmount); // Interpolate with the lerpAmount unclamped so it can extrapolate
    }

    private void InterpolateLocalRotation(float lerpAmount)
    {
        if (to.Rotation == previous.Rotation)
        {
            // If this object isn't supposed to be rotating, we don't want to interpolate and potentially extrapolate
            if (to.Rotation != from.Rotation)
            {
                // If this object hasn't reached it's intended local rotation
                transform.localRotation = Quaternion.Slerp(from.Rotation, to.Rotation, lerpAmount); // Interpolate with the lerpAmount clamped so no extrapolation occurs
            }
            return;
        }

        transform.localRotation = Quaternion.SlerpUnclamped(from.Rotation, to.Rotation, lerpAmount); // Interpolate with the lerpAmount unclamped so it can extrapolate
    }

    public void UpdateTransform(int tick, Vector3 position, Quaternion rotation)
    {
        if (tick <= GameManager.Instance.TickDelay)
        {
            return;
        }

        if (futureTransformUpdates.Count == 0)
        {
            futureTransformUpdates.Add(new TransformUpdate(tick, position, rotation));
            return;
        }

        for (int i = 0; i < futureTransformUpdates.Count; i++)
        {
            if (tick < futureTransformUpdates[i].Tick)
            {
                // Transform update is older
                futureTransformUpdates.Insert(i, new TransformUpdate(tick, position, rotation));
                break;
            }
        }
    }

    public void UpdateTransform(int tick, Vector3 position)
    {
        if (tick <= GameManager.Instance.TickDelay)
        {
            return;
        }

        if (futureTransformUpdates.Count == 0)
        {
            futureTransformUpdates.Add(new TransformUpdate(tick, position));
            return;
        }

        for (int i = 0; i < futureTransformUpdates.Count; i++)
        {
            if (tick < futureTransformUpdates[i].Tick)
            {
                // Position update is older
                futureTransformUpdates.Insert(i, new TransformUpdate(tick, position));
                break;
            }
        }
    }

    public void UpdateTransform(int tick, Quaternion rotation)
    {
        if (tick <= GameManager.Instance.TickDelay)
        {
            return;
        }

        if (futureTransformUpdates.Count == 0)
        {
            futureTransformUpdates.Add(new TransformUpdate(tick, rotation));
            return;
        }

        for (int i = 0; i < futureTransformUpdates.Count; i++)
        {
            if (tick < futureTransformUpdates[i].Tick)
            {
                // Rotation update is older
                futureTransformUpdates.Insert(i, new TransformUpdate(tick, rotation));
                break;
            }
        }
    }
}