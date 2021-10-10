using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NormalCharacterMotor : CharacterMotor
{
    public float MaxRotationSpeed = 270;

    private bool firstFrame = true;

    private void OnEnable()
    {
        firstFrame = true;
    }
    private void Update()
    {
        UpdateVelocity();
    }

    private void UpdateVelocity()
    {
        CharacterController controller = GetComponent(typeof(CharacterController)) as CharacterController;
        Vector3 velocity = controller.velocity;

        if (firstFrame)
        {
            velocity = Vector3.zero;
            firstFrame = false;
        }
        if (Grounded) velocity = Util.ProjectOntoPlane(velocity, transform.up);

        // Calculate how fast we should be moving
        Vector3 movement = velocity;

        //bool hasJumped = false;
        Jumping = false;

        if (Grounded)
        {
            // Apply a force that attempts to reach our target velocity
            Vector3 velocityChange = (DesiredVelocity - velocity);

            if (velocityChange.magnitude > MaxVelocityChange)
            {
                velocityChange = velocityChange.normalized * MaxVelocityChange;
            }
            movement += velocityChange;

            // Jump
            if (CanJump && Input.GetButton("Jump"))
            {
                movement += transform.up * Mathf.Sqrt(2 * JumpHeight * Gravity);
                //hasJumped = true;
                Jumping = true;
            }
        }

        // Apply downwards gravity
        movement += transform.up * -Gravity * Time.deltaTime;

        if (Jumping)
        {
            movement -= transform.up * -Gravity * Time.deltaTime / 2;

        }

        // Apply movement
        CollisionFlags flags = controller.Move(movement * Time.deltaTime);
        Grounded = (flags & CollisionFlags.CollidedBelow) != 0;
    }
}
