
using UnityEditor.UIElements;
using UnityEngine;

namespace PlayerController {
	public interface ILookController {
		Transform target {get;}
		bool firstPerson {get;}

		float Zoom {get; set;}

		void SetTarget(Transform target);
	}
}