using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController {
	public class FreeGravityController : MovementController {
		static Vector3 gravityUp => -Physics.gravity.normalized;

		bool shouldFlip => Vector3.Angle(transform.up, gravityUp) > settings.gravityDeadzone;

		protected override void Awake() {
			if (settings is null || camTransform is null)
				throw new ArgumentNullException();
			
			base.Awake();
		}

		protected virtual void Update() {
			/*var camForward = camTransform.forward;
			var camUp = Vector3.Project(camForward, -gravityUp);
			camForward += camUp;
			var camRight = Vector3.Project(camForward, Vector3.Cross(camForward, gravityUp));
			camForward += camRight;
			camForward.Normalize();
			camRight.Normalize();*/
			
			var camForward = camTransform.forward;
			camForward = Vector3.ProjectOnPlane(camForward, gravityUp);
			camForward.Normalize();
			var camRight = Vector3.Cross(gravityUp, camForward);
			camRight.Normalize();
			moveDirection = camForward * moveInput.y + camRight * moveInput.x;
		}

		void FixedUpdate() {
			if (moveDirection != Vector3.zero)
				AddMovement();
			else
				BreakMovement();
		}

		void AddMovement() {
			Vector3 force = moveDirection * settings.accel;

			rigidbody.AddForce(force);
			ClampVelocityHorizontal(settings.speed);

			AlignGroundedRotation(shouldFlip);
		}

		protected virtual void BreakMovement()
		{
			Vector3 flatVel = Flatten(rigidbody.velocity);

			if (flatVel.magnitude > settings.minSpeed)
				rigidbody.AddForce(-flatVel.normalized * settings.decel);
			else
				ClampVelocityHorizontal(0f);

			AlignGravityRotation();
		}

		void ClampVelocityHorizontal(float maxSpeed) {
			Vector3 flatVel = Flatten(rigidbody.velocity);
			var offset = rigidbody.velocity - flatVel;
			
			if (flatVel.magnitude > maxSpeed)
				flatVel = flatVel.normalized * maxSpeed;
			
			rigidbody.velocity = flatVel + offset;
		}

		void AlignGroundedRotation(bool flipping = false) {
			Quaternion goalRot = Quaternion.LookRotation(moveDirection, gravityUp);
			Quaternion slerp = Quaternion.Slerp(transform.rotation, goalRot, (flipping ? settings.gravityTurnSpeed : settings.turnSpeed) * moveDirection.magnitude * Time.fixedDeltaTime);

			transform.rotation = slerp;
		}

		void AlignGravityRotation() {
			Quaternion goalRot = Quaternion.LookRotation(transform.forward, gravityUp);
			Quaternion slerp = Quaternion.Slerp(transform.rotation, goalRot, settings.gravityTurnSpeed * Time.fixedDeltaTime);

			transform.rotation = slerp;
		}

		static Vector3 Flatten(Vector3 vector) => Vector3.ProjectOnPlane(vector, gravityUp);

		void OnMove(InputAction.CallbackContext ctx) {
			Vector2 input = ctx.ReadValue<Vector2>();
			float magnitude = input.magnitude;
	        if (magnitude < settings.moveDeathZone)
	        {
	            input = Vector3.zero;
	        }
	        else
	        {
	            input -= settings.moveDeathZone * Vector2.one;
	            input *= (1 / (1 - settings.moveDeathZone));
	            if (magnitude > 1f)
	                input.Normalize();
	        }

			moveInput = input;
		}

		void OnJump(InputAction.CallbackContext ctx) {
			if (ctx.ReadValueAsButton() && IsGrounded())
				Jump();
		}

		protected virtual void Jump() {
			rigidbody.AddForce(settings.jumpForce * gravityUp, ForceMode.Impulse);
		} 

		bool GroundRaycast(out RaycastHit hit) => Physics.Raycast(
			transform.position + transform.up * settings.groundCheckStartPoint,
			-transform.up,
			out hit,
			settings.groundCheckStartPoint + settings.groundCheckDist,
			~gameObject.layer
		);

		public bool IsGrounded() => GroundRaycast(out _);

		public void Teleport(Vector3 position) {
			rigidbody.velocity = Vector3.zero;

			rigidbody.MovePosition(position);
		}

		public void Teleport(Transform transform) {
			Teleport(transform.position);

			rigidbody.MoveRotation(transform.rotation);
		}
		
		//Debug line for ground check
		/*void OnDrawGizmos() {
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(transform.position + Vector3.down * -settings.groundCheckStartPoint, transform.position + Vector3.down * settings.groundCheckDist);
		}*/

		/*public void OnDrawGizmos() {
			var quat = Quaternion.LookRotation(transform.forward, gravityUp);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position + quat * (Vector3.right * 2f));
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, transform.position + quat * (Vector3.up * 2f));
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position, transform.position + quat * (Vector3.forward * 2f));
		}*/
	}
}