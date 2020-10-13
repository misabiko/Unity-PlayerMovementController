using UnityEngine;
using UnityEngine.InputSystem;

#if CINEMACHINE
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook))]
#endif
public class LookController : MonoBehaviour {
	public float camXSensitivity = 10f;
	public float camYSensitivity = 10f;
	public PlayerInput playerInput;
	
#if CINEMACHINE
	public PlayerInput playerInput;

	Vector2 lookInput;
	CinemachineFreeLook cam;
#else
	[HideInInspector]
	public bool cameraLocked = false;

	Transform camTransform;
	Vector2 lookInput;
	float xRotation;
#endif

	void Reset() => playerInput = GetComponent<PlayerInput>();

	void Awake() {
		playerInput.actions["Look"].started += OnLook;
		playerInput.actions["Look"].performed += OnLook;
		playerInput.actions["Look"].canceled += OnLook;
		
#if CINEMACHINE
		cam = GetComponent<CinemachineFreeLook>();
#else
		camTransform = GetComponentInChildren<Camera>().transform;
#endif
	}

	void OnLook(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();

	void Update() {
#if CINEMACHINE
		if (
			playerInput.actions["Look"].activeControl?.device is Mouse &&
			!Mouse.current.rightButton.isPressed
			) return;
		
		cam.m_XAxis.Value += camXSensitivity * lookInput.x;
		cam.m_YAxis.Value -= camYSensitivity * lookInput.y;
#else
		if (!cameraLocked) {
			transform.Rotate(Vector3.up, camXSensitivity * lookInput.x * Time.deltaTime);

			xRotation -= camYSensitivity * lookInput.y * Time.deltaTime;
			xRotation = Mathf.Clamp(xRotation, -90f, 90f);
			camTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
		}
#endif
	}
}