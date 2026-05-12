using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class InfectedRobotAgent : Agent
{
	public Transform target;

	public override void CollectObservations(VectorSensor sensor)
	{
		sensor.AddObservation(target.position);
	}

	public override void OnActionReceived(ActionBuffers actions)
	{
		float moveX = actions.ContinuousActions[0];
		float moveZ = actions.ContinuousActions[1];

		transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime;
	}

	private void OnTriggerEnter(Collider other)
	{
		AddReward(1f);
		EndEpisode();
	}
}
