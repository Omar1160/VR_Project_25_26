using Assets.Scripts;
using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    public float speed = 40f;
    public float lifeTime = 3f; // Destroys itself after 3 seconds if it hits nothing
    public float damage = 10f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Move forward constantly
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Simple check forward to see if it hits a collider
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, speed * Time.deltaTime))
        {
            // We hit something!
            Debug.Log("Laser hit: " + hit.transform.name);

            if(hit.collider.CompareTag("Hunter"))
            {
				NPCAgent hunter = hit.collider.GetComponent<NPCAgent>();
				if (hunter != null)
				{
					hunter.FreezeByLaser();
					Debug.Log("Hunter geraakt door laser! Bevriezen geactiveerd.");
				}
			}
            Destroy(gameObject); // Destroy the laser bolt on impact
        }
    }
}