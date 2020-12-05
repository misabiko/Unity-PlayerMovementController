using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController {
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(PlayerInput))]
	public class MovementController : MonoBehaviour {
		public MovementSettings settings;
		public Transform camTransform;
		public float turnSpeed = 5;

		public float speed = 3.5f;
		public float sprintSpeed = 5f;
		public float accel = 100f;
		public float deccel = 80f;
		public float jumpForce = 5f;

		public float slopeForce = 5f;

		bool firstPerson;

		protected new Rigidbody rigidbody;

		protected Vector2 moveInput;
		protected Vector3 moveDirection;

		const float groundCheckDist = 0.05f;
		const float groundCheckStartPoint = 0.1f; //How high inside the collider the ray should start

#if ENABLE_SPRINT
		bool sprinting;

		protected float Speed => sprinting ? sprintSpeed : speed;
#else
		protected float Speed => speed;
#endif

		void Reset() {
			rigidbody = GetComponent<Rigidbody>();

			if (Camera.main is Camera cam) {
				camTransform = cam.transform;

				if (camTransform.IsChildOf(transform))
					firstPerson = true;
			}
		}

		protected virtual void Awake() {
			rigidbody = GetComponent<Rigidbody>();

			SetActions(GetComponent<PlayerInput>());
		}

		void Update() {
			if (firstPerson)
				moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
			else {
				var camForward = camTransform.forward;
				camForward.y = 0;
				camForward.Normalize();

				moveDirection = camForward * moveInput.y + camTransform.right * moveInput.x;
			}
		}

		void FixedUpdate() {
			if (moveDirection != Vector3.zero)
				AddMovement();
			else
				BreakMovement();
		}

		protected virtual void SetActions(PlayerInput playerInput) {
			playerInput.actions["Move"].started += OnMove;
			playerInput.actions["Move"].performed += OnMove;
			playerInput.actions["Move"].canceled += OnMove;

			playerInput.actions["Jump"].performed += OnJump;

#if ENABLE_SPRINT
				playerInput.actions["Sprint"].started += OnSprint;
				playerInput.actions["Sprint"].performed += OnSprint;
				playerInput.actions["Sprint"].canceled += OnSprint;
#endif
		}

		void AddMovement() {
			Vector3 force = moveDirection * accel;

			rigidbody.AddForce(force);
			ClampVelocityXZ(Speed);

			if (!firstPerson)
				AlignGroundedRotation();

			if (OnSlope())
				rigidbody.AddForce(Vector3.down * slopeForce);
		}

		void BreakMovement() {
			Vector3 flatVel = Flatten(rigidbody.velocity);

			if (flatVel.magnitude > 0.2f)
				rigidbody.AddForce(-flatVel.normalized * deccel);
			else
				ClampVelocityXZ(0f);
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
			Quaternion slerp = Quaternion.Slerp(transform.rotation, goalRot, turnSpeed * moveDirection.magnitude * Time.fixedDeltaTime);

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
				rigidbody.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
		}

		bool IsGrounded() => GroundRaycast(out _);

		//if (isJumping) return false;
		bool OnSlope() => GroundRaycast(out RaycastHit hit) && hit.normal != Vector3.up;

		bool GroundRaycast(out RaycastHit hit) => Physics.Raycast(
			transform.position + Vector3.down * (1f - groundCheckStartPoint),
			Vector3.down,
			out hit,
			groundCheckStartPoint + groundCheckDist,
			~gameObject.layer
		);

#if ENABLE_SPRINT
		void OnSprint(InputAction.CallbackContext ctx) => sprinting = ctx.ReadValueAsButton();
#endif
	}
}