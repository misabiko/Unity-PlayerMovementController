using UnityEngine;

namespace PlayerController {
	[CreateAssetMenu]
	public class MovementSettings : ScriptableObject {
		public float jumpForce = 10f;

		[Range(0f, 1f)]
		public float aerialControl = 0.5f;

		public float speed = 3.5f;
		
		public float minSpeed = 3.5f;
		
		public float accel = 100f;
		
		public float decel = 80f;
		
		public float turnSpeed = 5;

		public float groundCheckDist = 0.05f;
		public float groundCheckStartPoint = 0.1f; //How high inside the collider the ray should start

        [Range(0f, 1f)]
        public float moveDeathZone = 0.05f;

        [Header("Anchor")]
		[Min(0f)]
		public float anchorTurnSpeed = 30f;

		[Header("Gravity Stuff")]
		public float gravityDeadzone = 3f;
		
		[Min(0f)]
		public float gravityTurnSpeed = 3f;
	}
}