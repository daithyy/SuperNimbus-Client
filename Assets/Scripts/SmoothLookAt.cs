using UnityEngine;

public class SmoothLookAt : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(2 * transform.position - target.position);
        transform.rotation = Quaternion.LookRotation(transform.up, target.forward);
    }
}
