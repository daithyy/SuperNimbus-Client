using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int Id;

    public string Username;

    public int ItemCount = 0;

    [Header("References")]
    [HideInInspector]
    public JumpController JumpController;

    [HideInInspector]
    public PlayerInterpolator Interpolator;
}
