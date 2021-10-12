using UnityEngine;

public class PlayerController : MonoBehaviour 
{
	public float WalkMultiplier = 0.5f;

	public bool DefaultIsWalk = false;

    private void FixedUpdate()
    {
		ReadInput();
	}

	private void ReadInput()
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
		ClientSend.PlayerMovement(directionVector, actions);
	}
}
