using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class OVRRigMoveSprintJump : MonoBehaviour
{
    public Transform head; // TrackingSpace/CenterEyeAnchor

    [Header("Collider")]
    public float height = 1.7f;
    public float radius = 0.25f;

    [Header("Move")]
    public float walkSpeed = 2.0f;
    public float sprintSpeed = 10.0f;

    [Header("Jump/Gravity")]
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    [Header("Simulation keys")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;

    CharacterController cc;
    float verticalVelocity;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        cc.height = height;
        cc.radius = radius;
        cc.center = new Vector3(0, height * 0.5f, 0);
    }

    void Update()
    {
        if (!head) return;

        // Keep controller under head (X/Z), fixed height
        Vector3 headLocal = transform.InverseTransformPoint(head.position);
        cc.height = height;
        cc.radius = radius;
        cc.center = new Vector3(headLocal.x, height * 0.5f, headLocal.z);

        // Movement (WASD) — reliable in Meta XR Simulation
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Optional: also allow thumbstick axis if it exists (device / some sim configs)
        Vector2 stick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        if (Mathf.Abs(stick.x) > 0.01f || Mathf.Abs(stick.y) > 0.01f)
        {
            x = stick.x;
            z = stick.y;
        }

        Vector3 forward = Vector3.ProjectOnPlane(head.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(head.right, Vector3.up).normalized;

        Vector3 move = (right * x + forward * z);
        if (move.sqrMagnitude > 1f) move.Normalize();

        // Sprint: key in sim, OR thumbstick click on device (if mapped)
        bool sprint =
            Input.GetKey(sprintKey) ||
            Input.GetKey(KeyCode.LeftShift) ||
            OVRInput.Get(OVRInput.Button.PrimaryThumbstick);

        float speed = sprint ? sprintSpeed : walkSpeed;

        // Jump: key in sim, OR A button on device (if mapped)
        bool jumpDown =
            Input.GetKeyDown(jumpKey) ||
            OVRInput.GetDown(OVRInput.Button.One);

        // Gravity + jump
        if (cc.isGrounded && verticalVelocity < 0f) verticalVelocity = -1f;

        if (cc.isGrounded && jumpDown)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * speed;
        velocity.y = verticalVelocity;

        cc.Move(velocity * Time.deltaTime);
    }
}
