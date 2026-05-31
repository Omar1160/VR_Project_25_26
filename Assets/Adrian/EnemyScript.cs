using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;
using System;

public class EnemyScript : Agent
{

    public static event Action OnCivilianCaught;
    public EnvironmentManager envManager;
    private float moveSpeed = 6f;

    private Rigidbody rb;


    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }


    public override void OnEpisodeBegin()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (envManager != null)
        {
            envManager.ResetRound(this.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.001f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Civilian"))
        {
            Debug.Log("Caught!");

            CivilianScript civilian = collision.gameObject.GetComponent<CivilianScript>();
            if (civilian != null)
            {
                civilian.SetReward(-0.5f);
                civilian.EndEpisode();
            }

            collision.gameObject.SetActive(false);

            
            AddReward(0.2f);

            envManager.activeCivilians--;

            if (envManager.activeCivilians <= 0)
            {
                AddReward(1.0f);
                EndEpisode(); // This automatically calls OnEpisodeBegin()!
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardInput = actions.ContinuousActions[0];
        float lateralInput = actions.ContinuousActions[1];


        Vector3 moveDirection = new Vector3(lateralInput, 0f, forwardInput).normalized;

        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);


        AddReward(-0.0001f);

        if (StepCount >= MaxStep - 1)
        {
            // Punish the hunter for failing to catch everyone in time
            AddReward(-0.5f);
            Debug.Log("Hunter failed to catch everyone in time!");
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var c = actionsOut.ContinuousActions;

        // Vooruit achteruit (W/S or Up/Down Arrow keys) -> maps to continuous actions index 0
        c[0] = Input.GetAxisRaw("Vertical");

        // Links rechts (A/D or Left/Right Arrow keys) -> maps to continuous actions index 1
        c[1] = Input.GetAxisRaw("Horizontal");
    }
}
