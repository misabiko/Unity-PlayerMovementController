using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject {
	public float speed;
	public float sprintSpeed;
	public float accel;
	public float deccel;
	public float turnSpeed;

	public float jumpForce;
}
