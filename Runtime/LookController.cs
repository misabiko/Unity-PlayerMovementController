using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController {
	public class LookController : MonoBehaviour, ILookController {
		public float camXSensitivity {get; set;}
		public float camYSensitivity {get; set;}

		[field: SerializeField]
		public float Zoom {get; set;} = 1f;
		public float zoomSensitivity {get; set;} = 0.1f;

		public bool cameraLocked = false;
		public PlayerInput playerInput;
		public float camDistance = 10f;

		public bool firstPerson {get; private set;}

		Transform camTransform;
		Vector2 lookInput;
		float xRotation;

		public Transform target => playerInput.transform;

		void Reset() {
			playerInput = GetComponent<PlayerInput>();

			if (firstPerson) {
				camXSensitivity = 10f;
				camYSensitivity = 10f;
			} else {
				camXSensitivity = 0.1f;
				camYSensitivity = 0.01f;
			}
		}

		void Awake() {
			playerInput.actions["Look"].started += OnLook;
			playerInput.actions["Look"].performed += OnLook;
			playerInput.actions["Look"].canceled += OnLook;


			playerInput.actions["Zoom"].started += OnZoom;
			playerInput.actions["Zoom"].performed += OnZoom;
			playerInput.actions["Zoom"].canceled += OnZoom;

			camTransform = GetComponentInChildren<Camera>().transform;
		}

		void OnLook(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();

		void OnZoom(InputAction.CallbackContext ctx) {
			camDistance += ctx.ReadValue<float>() * zoomSensitivity;
		}

		void Update() {
			if (!cameraLocked) {
				transform.Rotate(Vector3.up, camXSensitivity * lookInput.x * Time.deltaTime);

				xRotation -= camYSensitivity * lookInput.y * Time.deltaTime;
				xRotation = Mathf.Clamp(xRotation, -90f, 90f);
				camTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
			}
		}

		public void SetPerspective(bool firstPerson) => this.firstPerson = firstPerson;

		public void SetTarget(Transform target) => playerInput = target.GetComponent<PlayerInput>();
	}
}