using UnityEngine;

public class RotateSky : MonoBehaviour
{
    public float RotateSpeed = 1f;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", RotateSpeed * Time.time);
    }
}
