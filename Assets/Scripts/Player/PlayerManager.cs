using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int Id;

    public string Username;

    [Header("References")]
    [HideInInspector]
    public JumpController JumpController;
}
