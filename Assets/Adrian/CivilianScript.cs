using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class CivilianScript : Agent
{
    private float moveSpeed = 5f;

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
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.001f);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardInput = actions.ContinuousActions[0]; 
        float lateralInput = actions.ContinuousActions[1]; 


        Vector3 moveDirection = new Vector3(lateralInput, 0f, forwardInput).normalized;

        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);

        AddReward(0.001f);

        if (StepCount >= MaxStep - 1)
        {
            // The Grand Survival Prize!
            AddReward(0.5f);
            Debug.Log(" won by surviving the clock!");
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
