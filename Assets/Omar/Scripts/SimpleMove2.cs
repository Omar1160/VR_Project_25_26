using Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleMove2 : MonoBehaviour
{
    public bool isMovementFrozen = false;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;

    [Header("Look")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Key Interaction")]
    public float interactionDistance = 3f;
    private float holdTimer = 0f;
    public bool hasKey = false;

    [Header("Escape Port Settings")]
    public GameObject escapePort;
	public float moveDuration = 2.0f; // Tijd in seconden
	public float moveHeight = 31.0f;  // Hoe hoog hij gaat

	float xRotation = 0f;
    bool isGrounded;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Look();
        if (!isMovementFrozen)
        {
            GroundCheck();
            Jump();
            HandleInteraction();
        }
    }

    void HandleInteraction()
    {
        RaycastHit hit;
        if(Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactionDistance))
        {
            // 1. Sleutel oppakken
            if(hit.collider.CompareTag("Key") && !hasKey)
            {
                if(Input.GetKey(KeyCode.E))
                {
					Debug.Log($"Sleutel vasthouden... {holdTimer:F1}s");

					holdTimer += Time.deltaTime;
                    if(holdTimer >= 3.0f)
                    {
                        hasKey = true;
                        hit.collider.GetComponent<Key>().PickUp(playerCamera);
						Debug.Log("<color=green>SUCCESS: Sleutel is succesvol opgepakt!</color>");
					}
                }
                else { holdTimer = 0f; }
            }

            // 2. EscapePort openen
            if(hit.collider.CompareTag("EscapePort") && hasKey)
            {
				Debug.Log("Ik kijk naar de poort!");
				if (Input.GetKeyDown(KeyCode.E))
                {
					Debug.Log("<color=blue>ACTIE: EscapePort geactiveerd met de sleutel.</color>");
					OpenEscapePort(hit.collider.gameObject);
                }
            }

			// 3. EscapePort closen

            if(hit.collider.CompareTag("CloseButton"))
            {
                if(Input.GetKeyDown(KeyCode.C))
                {
                    Debug.Log("Poort sluiten..");
                    StartCoroutine(MovePortDown(escapePort));

                    SceneManager.LoadScene("GameWon");
                }
            }

			

		}
	}

    void OpenEscapePort(GameObject port)
    {
        // Start de animatie als Coroutine
        StartCoroutine(MovePortUp(port));

        // Kleur direct veranderen
        port.GetComponent<Renderer>().material.color = Color.yellow;

		NPCAgent[] allAgents = FindObjectsByType<NPCAgent>(FindObjectsSortMode.None);

		foreach (var agent in allAgents)
		{
			// Check of de agent "Freezed" is
			if (agent.CompareTag("Freezed"))
			{
				agent.UnfreezeAndEscape();
			}
		}
	}

    IEnumerator MovePortUp(GameObject port)
    {
        Vector3 startPos = port.transform.position;
        Vector3 targetPos = startPos + new Vector3(0, 31f, 0);
         moveDuration = 2.0f;
        float elapsed = 0f;

        while(elapsed < moveDuration)
        {
            // Lerp zorgt voor vloeiende overgang
            port.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;

        }

        port.transform.position = targetPos;
    }

	IEnumerator MovePortDown(GameObject port)
    {
        Vector3 startPos = port.transform.position;

        Vector3 targetPos = new Vector3(startPos.x, 0f, startPos.z);

        float elapsed = 0f;
        moveDuration = 2.0f;

        while (elapsed < moveDuration)
        {
            port.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        port.transform.position = targetPos;

        
    }


	void FixedUpdate()
    {
        if (!isMovementFrozen)
        {
            Move();
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        Vector3 velocity = move * currentSpeed;

        Vector3 newVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        rb.linearVelocity = newVelocity;
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );

		if (isGrounded)
		{
			Debug.Log("De grond is gedetecteerd!");
		}
	}

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
