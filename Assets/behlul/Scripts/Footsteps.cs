using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioSource footstepsSound;

    private void Update()
    {
        Vector2 moveInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        bool isWalking = moveInput.magnitude > 0.1f;

        if (isWalking)
        {
            if (!footstepsSound.isPlaying)
            {
                footstepsSound.Play();
            }
        }
        else
        {
            if (footstepsSound.isPlaying)
                footstepsSound.Pause();
        }
    }
}
