using UnityEngine;

public class PlayerController : MonoBehaviour 
{
	public float WalkMultiplier = 0.5f;

	public bool DefaultIsWalk = false;

	public static bool LockInput = false;

	private AimController aim;

	private void Start()
    {
		aim = gameObject.AddComponent<AimController>();
    }

    private void Update()
    {
		ReadInputLocal();

		if (!LockInput)
        {
			aim.UpdateView();
        }
    }

    private void FixedUpdate()
    {
		if (!LockInput)
        {
			ReadInputServer();
		}
	}

	private void ReadInputLocal()
	{
		if (Input.GetKeyUp(KeyCode.Y) && !UIManager.Instance.ChatInputField.isFocused)
		{
			LockInput = !LockInput;
			UIManager.Instance.ToggleChat(LockInput);
		}
	}

	private void ReadInputServer()
    {
		// Get input vector from keyboard or analog stick and make it length 1 at most
		Vector3 directionVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

		if (directionVector.magnitude > 1)
		{
			directionVector = directionVector.normalized;
        }

        if (WalkMultiplier != 1)
        {
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) != DefaultIsWalk)
            {
                directionVector *= WalkMultiplier;
            }
        }

		bool[] actions = new bool[]
		{
			Input.GetKey(KeyCode.Space),
		};

		// Send input direction to REMOTE server (Server Authoriative movement)
		SendController.PlayerMovement(directionVector, actions);
	}
}
