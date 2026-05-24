using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed = 3f;
	public float mouseSensitivity = 2f;

	float rotationX = 0f;

	private void Update()
	{
		float moveX = Input.GetAxis("Horizontal");
		float moveZ = Input.GetAxis("Vertical");

		transform.Translate(moveX * speed * Time.deltaTime, 0, moveZ * speed * Time.deltaTime);

		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
		transform.Rotate(0, mouseX, 0);
	}


}
