using UnityEngine;

public class FootStepsNM : MonoBehaviour
{
    public AudioSource footstepsSound;
    public AudioSource sprintSound;

    public float moveThreshold = 0.1f;
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        bool isMoving = (x * x + z * z) > (moveThreshold * moveThreshold);

        bool isSprinting = isMoving && Input.GetKey(KeyCode.LeftShift);

        if (isSprinting)
        {
            if (footstepsSound.isPlaying) footstepsSound.Pause();
            if (!sprintSound.isPlaying) sprintSound.Play();
        }
        else if (isMoving)
        {
            if (sprintSound.isPlaying) sprintSound.Pause();
            if (!footstepsSound.isPlaying) footstepsSound.Play();
        }
        else
        {
            if (footstepsSound.isPlaying) footstepsSound.Pause();
            if (sprintSound.isPlaying) sprintSound.Pause();
        }
    }
}
