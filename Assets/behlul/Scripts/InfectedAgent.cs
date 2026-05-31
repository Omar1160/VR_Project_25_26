using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class InfectedAgent : Agent
{
    public float speedMultiplier = 0.1f;
    public float rotationMultiplier = 5f;
    public override void OnEpisodeBegin()
    {

    }
    public override void CollectObservations(VectorSensor sensor)
    {

    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.z = actionBuffers.ContinuousActions[0];
        transform.Translate(controlSignal * speedMultiplier);

        transform.Rotate(0.0f, rotationMultiplier * actionBuffers.ContinuousActions[1], 0.0f);

    }

    //public override void Heuristic(in ActionBuffers actionsOut)
    //{
    //    var c = actionsOut.ContinuousActions;

    //    // Vooruit achteruit
    //    c[0] = Input.GetAxis("Vertical");

    //    // Links rechts
    //    c[1] = Input.GetAxis("Horizontal");
    //}
}
