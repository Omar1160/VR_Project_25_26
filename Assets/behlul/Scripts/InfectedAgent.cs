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
        HealthyAgent nearest = FindNearestHealthy();

        Vector3 dir =
            (nearest.transform.position - transform.position).normalized;

        float dist =
            Vector3.Distance(transform.position, nearest.transform.position);

        sensor.AddObservation(dir);
        sensor.AddObservation(dist);
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.z = actionBuffers.ContinuousActions[0];
        transform.Translate(controlSignal * speedMultiplier);

        transform.Rotate(0.0f, rotationMultiplier * actionBuffers.ContinuousActions[1], 0.0f);

        AddReward(-0.0005f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HealthyAgent healthy =
            collision.gameObject.GetComponent<HealthyAgent>();

        if (healthy != null)
        {
            healthy.Infected();

            AddReward(1f);
        }
    }
    public void InfectHealthy(HealthyAgent healthy)
    {
        AddReward(1f);

        healthy.Infected();
    }

    private HealthyAgent FindNearestHealthy()
    {
        HealthyAgent[] healthyAgents =
            FindObjectsOfType<HealthyAgent>();

        HealthyAgent nearest = null;

        float closestDistance = Mathf.Infinity;

        foreach (HealthyAgent healthy in healthyAgents)
        {
            float distance =
                Vector3.Distance(transform.position,
                                 healthy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearest = healthy;
            }
        }

        return nearest;
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
