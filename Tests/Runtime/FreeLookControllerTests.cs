using NUnit.Framework;
using PlayerController;
using UnityEngine;

namespace Tests {
	public class FreeLookControllerTests : ILookControllerTestsBase {
		[OneTimeSetUp]
		public void Init() {
			var gameObject = new GameObject();
			controller = gameObject.AddComponent<FreeLookController>();
			controller.SetTarget(target);
		}
		
	}
}