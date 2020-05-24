using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour {
	public Transform camTransform;
	public PlayerData data;

	new Rigidbody rigidbody;

	Vector2 moveInput;
	Vector3 moveDirection;
	bool sprinting;

	void Awake() {
		rigidbody = GetComponent<Rigidbody>();

		Camera cam = Camera.main;
		if (cam != null)
			camTransform = cam.transform;

		var playerInput = GetComponent<PlayerInput>();

		playerInput.actions["Move"].started += OnMove;
		playerInput.actions["Move"].performed += OnMove;
		playerInput.actions["Move"].canceled += OnMove;

		playerInput.actions["Jump"].performed += OnJump;

		playerInput.actions["Sprint"].started += OnSprint;
		playerInput.actions["Sprint"].performed += OnSprint;
		playerInput.actions["Sprint"].canceled += OnSprint;
	}

	void Update() {
		Vector3 camForward = camTransform.forward;
		camForward.y = 0;
		camForward.Normalize();

		moveDirection = camForward * moveInput.y + camTransform.right * moveInput.x;
	}

	void FixedUpdate() {
		if (moveDirection != Vector3.zero) {
			Vector3 force = moveDirection * data.accel;

			rigidbody.AddForce(force);
			ClampVelocityXZ(sprinting ? data.sprintSpeed : data.speed);

			AlignGroundedRotation();
		}else {
			Vector3 flatVel = Flatten(rigidbody.velocity);
			
			if (flatVel.magnitude > 0.2f)
				rigidbody.AddForce(-flatVel.normalized * data.deccel);
			else
				ClampVelocityXZ(0f);
		}
	}

	void ClampVelocityXZ(float maxSpeed) {
		Vector3 flatVel = rigidbody.velocity;
		flatVel.y = 0;

		if (flatVel.magnitude > maxSpeed)
			flatVel = flatVel.normalized * maxSpeed;
		flatVel.y = rigidbody.velocity.y;
		rigidbody.velocity = flatVel;
	}

	void AlignGroundedRotation() {
		Quaternion goalRot = Quaternion.LookRotation(moveDirection);
		Quaternion slerp = Quaternion.Slerp(transform.rotation, goalRot, data.turnSpeed * moveDirection.magnitude * Time.fixedDeltaTime);

		transform.rotation = slerp;
	}

	static Vector3 Flatten(Vector3 vector) {
		vector.y = 0f;
		return vector;
	}

	void OnMove(InputAction.CallbackContext ctx) {
		Vector2 input = ctx.ReadValue<Vector2>();
		float magnitude = input.magnitude;
		if (magnitude > 1f)
			input.Normalize();

		moveInput = input;
	}

	void OnJump(InputAction.CallbackContext ctx) {
		if (IsGrounded())
			rigidbody.AddForce(data.jumpForce * Vector3.up, ForceMode.Impulse);
	}

	bool IsGrounded() => true;

	void OnSprint(InputAction.CallbackContext ctx) => sprinting = ctx.ReadValueAsButton();
}