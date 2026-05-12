using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class HealthyRobotAgent : Agent
{

	public Transform infectedRobot;

	public override void CollectObservations(VectorSensor sensor)
	{
		sensor.AddObservation(transform.position);
		sensor.AddObservation(infectedRobot.position);
	}

	public override void OnActionReceived(ActionBuffers actions)
	{
		float moveX = actions.ContinuousActions[0];
		float moveZ = actions.ContinuousActions[1];

		transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime;

		float distance = Vector3.Distance(transform.position, infectedRobot.position);

		if(distance > 5f)
		{
			AddReward(0.01f);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		AddReward(-1f);
		EndEpisode();
	}
}
