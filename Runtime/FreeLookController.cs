#if CINEMACHINE_ENABLE
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController {
	[RequireComponent(typeof(CinemachineFreeLook))]
	public class FreeLookController : MonoBehaviour, ILookController {
		public float camXSensitivity {get; set;}
		public float camYSensitivity {get; set;}

		const float minZoom = 0.2f;
		
		[Min(minZoom)]
		[SerializeField]
		float _zoom = 1f;
		public float Zoom {
			get => _zoom;
			set {
				_zoom = Mathf.Max(minZoom, value);
				
				for (int i = 0; i < virtualCam.m_Orbits.Length; i++)
				{
					virtualCam.m_Orbits[i].m_Height = originalOrbits[i].m_Height * (1f / _zoom);
					virtualCam.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * (1f / _zoom);
				}
			}
		}

		[field: SerializeField]
		public float ZoomSensitivity {get; set;} = 0.0001f;

		public bool cameraLocked = false;
		public PlayerInput playerInput;
		public float camDistance = 10f;

		public bool firstPerson => false;

		protected Transform camTransform;
		protected Vector2 lookInput;
		protected float xRotation;

		public CinemachineFreeLook.Orbit[] originalOrbits = new CinemachineFreeLook.Orbit[0];

		CinemachineFreeLook virtualCam;

		public Transform target {
			get => virtualCam.Follow;
			set {
				virtualCam.Follow = value;
				virtualCam.LookAt = value;
			}
		}

		void Awake() {
			virtualCam = GetComponent<CinemachineFreeLook>();

			if (playerInput)
				SetInputCallbacks();

			if (originalOrbits.Length == 0)
				originalOrbits = virtualCam.m_Orbits;
		}

		void SetInputCallbacks() {
			playerInput.actions["Zoom"].started += OnZoom;
			playerInput.actions["Zoom"].performed += OnZoom;
			playerInput.actions["Zoom"].canceled += OnZoom;
		}

		void Update() {
			if (
				playerInput.actions["Look"].activeControl?.device is Mouse &&
				!Mouse.current.rightButton.isPressed
			) return;

			virtualCam.m_XAxis.Value += camXSensitivity * lookInput.x;
			virtualCam.m_YAxis.Value -= camYSensitivity * lookInput.y;
		}

		public void SetTarget(Transform target) => this.target = target;

		void OnZoom(InputAction.CallbackContext ctx) => Zoom += ctx.ReadValue<float>() * ZoomSensitivity;
	}
}
#endif