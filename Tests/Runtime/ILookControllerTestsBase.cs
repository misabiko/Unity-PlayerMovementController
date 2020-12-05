using NUnit.Framework;
using PlayerController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tests {
	public abstract class ILookControllerTestsBase {
		protected ILookController controller;
		protected Transform target;

		protected float targetDistance => Vector3.Distance(((MonoBehaviour) controller).transform.position, controller.target.position);

		[OneTimeSetUp]
		public void InitTarget() {
			var gameObject = new GameObject();
			target = gameObject.transform;
		}

		[Test]
		public void Target() {
			Assert.AreSame(target, controller.target);
		}

		protected void FirstPerson() {
			//controller.SetFirstPerson();

			Assert.IsTrue(controller.firstPerson);

			Assert.AreEqual(0f, targetDistance);
		}

		protected void ThirdPerson() {
			//controller.SetThirdPerson();

			Assert.IsFalse(controller.firstPerson);

			Assert.Greater(0f, targetDistance);
		}

		[Test]
		public void ZoomIn() {
			var initialDistance = targetDistance;
			
			controller.Zoom += 1f;
			Assert.Less(targetDistance, initialDistance);
		}

		[Test]
		public void ZoomOut() {
			var initialDistance = targetDistance;
			
			controller.Zoom -= 1f;
			Assert.Greater(targetDistance, initialDistance);
		}
	}
}