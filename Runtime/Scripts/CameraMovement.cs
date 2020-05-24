using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CameraMovement : MonoBehaviour {
	public PlayerInput playerInput;
	public float camXSensitivity;
	public float camYSensitivity;

	Vector2 lookInput;
	CinemachineFreeLook cam;

	void Awake() {
		cam = GetComponent<CinemachineFreeLook>();
		playerInput.actions["Look"].started += OnLook;
		playerInput.actions["Look"].performed += OnLook;
		playerInput.actions["Look"].canceled += OnLook;
	}

	void OnLook(InputAction.CallbackContext context) => lookInput = context.ReadValue<Vector2>();

	void Update() {
		if (
			playerInput.actions["Look"].activeControl?.device is Mouse &&
			!Mouse.current.rightButton.isPressed
			) return;
		
		cam.m_XAxis.Value += camXSensitivity * lookInput.x;
		cam.m_YAxis.Value -= camYSensitivity * lookInput.y;
	}
}