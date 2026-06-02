using UnityEngine;

public class Key : MonoBehaviour
{

    public float pickupTim = 3.0f;
    public bool isCollected = false;
    public AudioSource unlockSound;
  

    public void PickUp(Transform playerCamera)
    {
        isCollected = true;
        if (unlockSound != null) unlockSound.Play();

        // Maak de sleutel een kind van de camera
        transform.SetParent(playerCamera);

       
            transform.localPosition = new Vector3(0.18f, -0.17f, 0.41f);
			transform.localRotation = Quaternion.Euler(0, -41.17f, 86.2f);
			transform.localScale = new Vector3(1f, 1f, 1f);
	 

            // Schakel de collider uit zodat hij niet tegen de speler bottst
            GetComponent<Collider>().enabled = false;

    }

   
}
