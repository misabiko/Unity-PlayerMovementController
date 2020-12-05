using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController {
	public class CursorManager : MonoBehaviour {
		public InputAction pauseAction;
		public LookController lookController;

		void Start() {
			pauseAction.performed += _ => FreeCursor();

			CaptureCursor();
		}

		void OnEnable() => pauseAction.Enable();

		void OnDisable() => pauseAction.Disable();

		public void CaptureCursor() {
			Cursor.lockState = CursorLockMode.Locked;
			lookController.cameraLocked = false;
		}

		public void FreeCursor() {
			Cursor.lockState = CursorLockMode.None;
			lookController.cameraLocked = true;
		}

		void OnApplicationFocus(bool hasFocus) {
			if (hasFocus)
				CaptureCursor();
			else
				FreeCursor();
		}
	}
}