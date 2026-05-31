using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class HealthyAgent : Agent
{
    public Transform exitZone;
    public float speedMultiplier = 0.1f;
    public float rotationMultiplier = 5f;
    public override void OnEpisodeBegin()
    {

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        InfectedAgent nearest = FindNearestInfected();

        Vector3 dirToInfected =
            (nearest.transform.position - transform.position).normalized;

        float distance =
            Vector3.Distance(transform.position, nearest.transform.position);

        Vector3 dirToExit =
            (exitZone.position - transform.position).normalized;

        sensor.AddObservation(dirToInfected);
        sensor.AddObservation(distance);

        sensor.AddObservation(dirToExit);
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.z = actionBuffers.ContinuousActions[0];
        transform.Translate(controlSignal * speedMultiplier);

        transform.Rotate(0.0f, rotationMultiplier * actionBuffers.ContinuousActions[1], 0.0f);

        AddReward(0.001f);

    }

    public void Infected()
    {
        AddReward(-1f);
        EndEpisode();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Exit"))
        {
            AddReward(2f);
            EndEpisode();
        }
    }

    private InfectedAgent FindNearestInfected()
    {
        InfectedAgent[] infectedAgents =
            FindObjectsOfType<InfectedAgent>();

        InfectedAgent nearest = null;

        float closestDistance = Mathf.Infinity;

        foreach (InfectedAgent infected in infectedAgents)
        {
            float distance =
                Vector3.Distance(transform.position,
                                 infected.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearest = infected;
            }
        }

        return nearest;
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var c = actionsOut.ContinuousActions;

        // Vooruit achteruit
        c[0] = Input.GetAxis("Vertical");

        // Links rechts
        c[1] = Input.GetAxis("Horizontal");
    }
}
