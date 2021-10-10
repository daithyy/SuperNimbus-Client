using UnityEngine;

public class FPSCharacterController : MonoBehaviour 
{
	public float WalkMultiplier = 0.5f;

	public bool DefaultIsWalk = false;

	[HideInInspector]
	public CharacterMotor motor;

	// Use this for initialization
	void Start () 
	{
		motor = GetComponent(typeof(CharacterMotor)) as CharacterMotor;

		if (motor == null) 
			Debug.Log("Character Motor Controller is NULL");

		motor.DesiredFacingDirection = transform.forward;
	}

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

		bool[] moveDir = new bool[]
		{
			directionVector.x.Equals(1),
			directionVector.x.Equals(0),
			directionVector.z.Equals(1),
			directionVector.z.Equals(0)
		};

        if (WalkMultiplier != 1)
        {
            if ((Input.GetKey("left shift") || Input.GetKey("right shift")) != DefaultIsWalk)
            {
                directionVector *= WalkMultiplier;
            }
        }

        // Apply velocity in the LOCAL movement direction
        motor.DesiredMovementDirection = directionVector;

		// Send velocity to REMOTE movement direction (Server Authoriative movement)
		//ClientSend.PlayerMovement(moveDir);
	}
}
