using Assets.Scripts;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;


public class PreyAgent : Agent
{
	public Transform exitZone;

	public NPCAgent[] npcs;

	public float rotationSpeed = 150f;

	private float previousDistanceToExit;

	public float moveSpeed = 3f;

	private RobotAnimator robotAnimator;

	private Rigidbody rb;

	private Renderer rend;

	public Transform startPosition;


	public override void Initialize()
	{
		rb = GetComponent<Rigidbody>();

		rend = GetComponentInChildren<Renderer>();

		rend.material.color = Color.white;

		robotAnimator = GetComponent<RobotAnimator>();
	}

	public override void OnEpisodeBegin()
	{
		if (StepCount >= MaxStep)
		{
			Debug.Log("Episode ended: Max steps reached.");
		}

		transform.position = startPosition.position;

		rb.linearVelocity = Vector3.zero;

		npcs = FindObjectsByType<NPCAgent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);


	}
	public override void CollectObservations(VectorSensor sensor)
	{
		sensor.AddObservation(transform.position);

		sensor.AddObservation(exitZone.position);

		
	}


	public override void OnActionReceived(ActionBuffers actions)
	{
		float moveX = actions.ContinuousActions[0];
		float moveZ = actions.ContinuousActions[1];

		rb.linearVelocity = new Vector3(moveX * moveSpeed, rb.linearVelocity.y, moveZ * moveSpeed);

		// tiny survival reward
		AddReward(0.0001f);

		// Reward dichtbij exit

		float currentDistance = Vector3.Distance(transform.position, exitZone.position);

		float reward = previousDistanceToExit - currentDistance;

		AddReward(reward * 0.02f);

		previousDistanceToExit = currentDistance;

		// straf dichtbij infected

		foreach (NPCAgent npc in npcs)
		{
			if (!npc.isInfected)
				continue;
			float dist =
				Vector3.Distance(transform.position, npc.transform.position);

			if (dist < 3f)
				AddReward(-0.005f);
		}

		// rotation
		Vector3 move = new Vector3(moveX, 0, moveZ);
		if (move != Vector3.zero)
		{
			Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
			transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
		}

	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{
		var actions = actionsOut.ContinuousActions;

		float moveX = 0;
		float moveZ = 0;

		// WASD
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			moveZ = 1;

		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			moveZ = -1;

		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			moveX = -1;

		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			moveX = 1;

		actions[0] = moveX;
		actions[1] = moveZ;

		float turn = 0;

		if (Input.GetKey(KeyCode.Q))
			turn = -1;

		if (Input.GetKey(KeyCode.E))
			turn = 1;

		transform.Rotate(0, turn * rotationSpeed * Time.deltaTime, 0);
	}

	private void OnTriggerEnter(Collider other)
	{

		// Escape zone

		if(other.CompareTag("Escapezone"))
		{
			AddReward(10f);
			ResetEnvironment();
			Debug.Log("PREY ESCAPED");

			// GameManager.Instance.WinGame();
		}

	}

	private void ResetEnvironment()
	{
		// 1.Reset de PreyAgent zelf
		EndEpisode();

		// 2. Zoek alle NPCAgents in de scene en reset hen
		NPCAgent[] allNPCs = FindObjectsByType<NPCAgent>(FindObjectsSortMode.None);
		foreach (var npc in allNPCs)
		{
			npc.EndEpisode(); // Dit triggert de OnEpisodeBegin van elke NPC
		}
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Wall"))
		{
			Debug.Log("Hit wall");

			AddReward(-0.01f);
		}
	}


	public void GameOver()
	{
		AddReward(-5f);
		Debug.Log("Episode ended:");
		EndEpisode();
	}


}
