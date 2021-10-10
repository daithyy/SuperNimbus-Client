using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PhysicsCharacterMotor : CharacterMotor {
	
	public float maxRotationSpeed = 270;
	public bool useCentricGravity = false;
	public LayerMask groundLayers;
	public Vector3 gravityCenter = Vector3.zero;
	
	void Awake () {
		GetComponent<Rigidbody>().freezeRotation = true;
		GetComponent<Rigidbody>().useGravity = false;
	}
	
	private void AdjustToGravity() {
		int origLayer = gameObject.layer;
		gameObject.layer = 2;
		
		Vector3 currentUp = transform.up;
		//Vector3 gravityUp = (transform.position-gravityCenter).normalized;
		
		float damping = Mathf.Clamp01(Time.deltaTime*5);
		
		RaycastHit hit;
		
		Vector3 desiredUp = Vector3.zero;
		for (int i=0; i<8; i++) {
			Vector3 rayStart =
				transform.position
					+ transform.up
					+ Quaternion.AngleAxis(360*i/8.0f, transform.up)
						* (transform.right*0.5f)
					+ DesiredVelocity*0.2f;
			if ( Physics.Raycast(rayStart, transform.up*-2, out hit, 3.0f, groundLayers.value) ) {
				desiredUp += hit.normal;
			}
		}
		desiredUp = (currentUp+desiredUp).normalized;
		Vector3 newUp = (currentUp+desiredUp*damping).normalized;
		
		float angle = Vector3.Angle(currentUp,newUp);
		if (angle>0.01) {
			Vector3 axis = Vector3.Cross(currentUp,newUp).normalized;
			Quaternion rot = Quaternion.AngleAxis(angle,axis);
			transform.rotation = rot * transform.rotation;
		}
		
		gameObject.layer = origLayer;
	}
	
	private void UpdateFacingDirection() {
		// Calculate which way character should be facing
		float facingWeight = DesiredFacingDirection.magnitude;
		Vector3 combinedFacingDirection = (
			transform.rotation * DesiredMovementDirection * (1-facingWeight)
			+ DesiredFacingDirection * facingWeight
		);
		combinedFacingDirection = Util.ProjectOntoPlane(combinedFacingDirection, transform.up);
		combinedFacingDirection = AlignCorrection * combinedFacingDirection;
		
		if (combinedFacingDirection.sqrMagnitude > 0.1f) {
			Vector3 newForward = Util.ConstantSlerp(
				transform.forward,
				combinedFacingDirection,
				maxRotationSpeed*Time.deltaTime
			);
			newForward = Util.ProjectOntoPlane(newForward, transform.up);
			//Debug.DrawLine(transform.position, transform.position+newForward, Color.yellow);
			Quaternion q = new Quaternion();
			q.SetLookRotation(newForward, transform.up);
			transform.rotation = q;
		}
	}
	
	private void UpdateVelocity() {
		Vector3 velocity = GetComponent<Rigidbody>().velocity;
		if (Grounded) velocity = Util.ProjectOntoPlane(velocity, transform.up);
		
		// Calculate how fast we should be moving
		Jumping = false;
		if (Grounded) {
			// Apply a force that attempts to reach our target velocity
			Vector3 velocityChange = (DesiredVelocity - velocity);
			if (velocityChange.magnitude > MaxVelocityChange) {
				velocityChange = velocityChange.normalized * MaxVelocityChange;
			}
			GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);
		
			// Jump
			if (CanJump && Input.GetButton("Jump")) {
				GetComponent<Rigidbody>().velocity = velocity + transform.up * Mathf.Sqrt(2 * JumpHeight * Gravity);
				Jumping = true;
			}
		}
		
		// Apply downwards gravity
		GetComponent<Rigidbody>().AddForce(transform.up * -Gravity * GetComponent<Rigidbody>().mass);
		
		Grounded = false;
	}
	void OnCollisionStay () {
		Grounded = true;
	}
	
	void FixedUpdate () {
		if (useCentricGravity) AdjustToGravity();
		
		UpdateFacingDirection();
		
		UpdateVelocity();
	}
	
}
