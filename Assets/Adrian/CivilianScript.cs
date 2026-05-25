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

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardInput = actions.ContinuousActions[0]; 
        float lateralInput = actions.ContinuousActions[1]; 


        Vector3 moveDirection = new Vector3(lateralInput, 0f, forwardInput).normalized;

        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
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
