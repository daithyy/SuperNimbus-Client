using UnityEngine;

public class TrackPosition : MonoBehaviour
{
    public Transform Track;

    public float Offset = 1.2f;

    private Transform cachedTransform;

    private Vector3 cachedPosition;

    void Start()
    {
        cachedTransform = GetComponent<Transform>();

        if (Track)
        {
            cachedPosition = Track.position + new Vector3(0, Offset);
        }
    }

    void Update()
    {
        if (Track && cachedPosition != Track.position)
        {
            cachedPosition = Track.position + new Vector3(0, Offset);
            transform.position = cachedPosition;
        }
    }
}
