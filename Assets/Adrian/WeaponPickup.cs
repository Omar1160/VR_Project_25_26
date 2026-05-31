using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Arcade Floating Settings")]
    public float rotationSpeed = 50f;
    public float floatAmplitude = 0.2f; // How high it bobs up and down
    public float floatFrequency = 1f;   // How fast it bobs up and down
    private Vector3 startPosition;

    [Header("Settings")]
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.G;
    public float dropForwardForce = 5f;
    public float dropUpwardForce = 2f;

    [Header("UI Reference")]
    public GameObject interactionUI;

    [Header("State")]
    private bool isEquipped = false;
    private bool hasBeenDropped = false; // Tracks if it has been thrown yet
    private bool playerInRange = false;

    [Header("References")]
    private Transform playerWeaponSlot;
    private Rigidbody rb;
    private BoxCollider[] colliders;

    public GameObject laserPrefab; // Drag your LaserBolt prefab here
    public Transform firePoint;    // Drag an empty child object placed at the tip of the barrel here
    public float fireRate = 0.25f;  // Time between shots
    private float nextTimeToFire = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponents<BoxCollider>();

        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        // Save the starting position for the floating math
        startPosition = transform.position;

        // START AS FLOATING: Turn off physics gravity so it stays in the air
        rb.useGravity = false;
        rb.isKinematic = true;

        if (interactionUI != null) interactionUI.SetActive(true);
    }

    void Update()
    {
        // 1. COD Spinning & Floating Effect (Only happens if it hasn't been dropped yet)
        if (!isEquipped && !hasBeenDropped)
        {
            // Spin the gun around the Y axis
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

            // Smooth bobbing up and down using a Sine Wave
            Vector3 tempPos = startPosition;
            tempPos.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            transform.position = tempPos;
        }

        // --- NEW: SHOOTING LOGIC ---
        // Only allow shooting if the gun IS EQUIPPED and player clicks Left Mouse (Fire1)
        if (isEquipped && Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            ShootLaser();
        }


        // 2. Pickup Logic
        if (playerInRange && !isEquipped && Input.GetKeyDown(pickupKey))
        {
            PickupWeapon();
        }

        // 3. Drop Logic
        if (isEquipped && Input.GetKeyDown(dropKey))
        {
            DropWeapon();
        }

        // Rotate UI to always face the camera nicely
        if (!isEquipped && interactionUI != null && Camera.main != null)
        {
            interactionUI.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }
    }

    void ShootLaser()
    {
        if (firePoint != null && laserPrefab != null)
        {
            // Spawn the laser bolt at the barrel tip, matching the rotation of the barrel
            Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
        }
    }

    void PickupWeapon()
    {
        isEquipped = true;
        playerInRange = false;

        if (interactionUI != null) interactionUI.SetActive(false);

        rb.isKinematic = true;
        rb.useGravity = false;

        foreach (var col in colliders) col.enabled = false;

        transform.SetParent(playerWeaponSlot);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    void DropWeapon()
    {
        isEquipped = false;
        hasBeenDropped = true; // Tell the script to STOP floating/spinning now!

        if (interactionUI != null) interactionUI.SetActive(true);

        transform.SetParent(null);

        // ACTIVATE REAL PHYSICS: Now it falls like a normal item
        rb.isKinematic = false;
        rb.useGravity = true;

        foreach (var col in colliders) col.enabled = true;

        Vector3 forceDirection = playerWeaponSlot.forward * dropForwardForce + playerWeaponSlot.up * dropUpwardForce;
        rb.AddForce(forceDirection, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isEquipped)
        {
            playerInRange = true;
            playerWeaponSlot = other.transform.Find("WeaponSlot");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}   