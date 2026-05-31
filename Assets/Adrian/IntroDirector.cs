using System.Collections;
using UnityEngine;

public class IntroDirector : MonoBehaviour
{

    public SimpleMove playerScript;

    [Header("Settings")]
    public float freezeDuration = 10f;

    [Header("Meteor Settings")]
    public GameObject meteorObject;
    public Transform impactTarget;

    [Header("Timing")]
    public float soundAndSpawnDelay = 6f; // Time before meteor appears/sound starts
    public float fallDuration = 4f;       // How long the meteor takes to travel to the ground

    private AudioSource meteorAudio;
    private Vector3 startPosition;
    private float timeElapsed = 0f;
    private bool isMeteorFalling = false;

    [Header("Flash Settings")]
    public CanvasGroup flashCanvasGroup;
    public float flashFadeDuration = 2.5f;
    public AudioSource flashAudio;



    void Start()
    {
        if (playerScript == null || meteorObject == null || impactTarget == null)
        {
            Debug.LogError("SimpleIntroDirector: Please assign all references!");
            return;
        }

        if (flashCanvasGroup != null)
        {
            flashCanvasGroup.alpha = 0f;
        }   



        // Get the audio source from the meteor
        meteorAudio = meteorObject.GetComponent<AudioSource>();
        if (meteorAudio == null)
        {
            Debug.LogError("SimpleIntroDirector: No AudioSource component found on the Meteor object!");
        }
        else if (meteorAudio.clip == null)
        {
            Debug.LogWarning("SimpleIntroDirector: The AudioSource on the Meteor has NO AudioClip assigned!");
        }
        // Save where the meteor starts in the sky
        startPosition = meteorObject.transform.position;

        StartCoroutine(IntroSequenceRoutine());
    }

    IEnumerator IntroSequenceRoutine()
    {

        // 2. Wait 6 seconds looking around in silence
        yield return new WaitForSeconds(soundAndSpawnDelay);

        // 3. Make meteor appear and explicitly play the audio once
        meteorObject.SetActive(true);
        if (meteorAudio != null && meteorAudio.clip != null)
        {
            // FORCE 2D temporarily so distance doesn't mute it
            meteorAudio.spatialBlend = 0f;

            meteorAudio.Play();
            Debug.Log($" [AUDIO] Playing sound clip: {meteorAudio.clip.name} as 2D sound.");
        }
        else
        {
            Debug.LogError(" [AUDIO] Cannot play! AudioSource or Clip is missing.");
        }

        isMeteorFalling = true;
        Debug.Log("Meteor falling and audio playing!");
    }

    void Update()
    {
        if (isMeteorFalling && meteorObject != null)
        {
            // Smoothly move from start position to impact target over exactly 'fallDuration' seconds
            timeElapsed += Time.deltaTime;
            float percentageComplete = timeElapsed / fallDuration;

            meteorObject.transform.position = Vector3.Lerp(startPosition, impactTarget.position, percentageComplete);

            // Once it reaches the ground (percentage hits 1)
            if (percentageComplete >= 1f)
            {
                isMeteorFalling = false;
                Debug.Log("Meteor Impacted!");
                TriggerScreenFlash();
            }
        }
    }

    void TriggerScreenFlash()
    {
        if (playerScript != null)
        {
            playerScript.isMovementFrozen = true;
            Debug.Log("Impact shockwave! Player frozen.");
        }

        if (flashAudio != null)
        {
            flashAudio.Play();
        }

        if (flashCanvasGroup != null)
        {
            flashCanvasGroup.alpha = 1f; // Go full blind white instantly
            StartCoroutine(FadeFlashRoutine());
        }

        StartCoroutine(UnfreezePlayerRoutine());    
    }

    IEnumerator FadeFlashRoutine()
    {
        float elapsed = 0f;
        while (elapsed < flashFadeDuration)
        {
            elapsed += Time.deltaTime;
            flashCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / flashFadeDuration);
            yield return null;
        }
        flashCanvasGroup.alpha = 0f;
    }

    IEnumerator UnfreezePlayerRoutine()
    {
        // Wait for the exact freeze duration you set in the inspector (default 5s)
        yield return new WaitForSeconds(freezeDuration);

        if (playerScript != null)
        {
            playerScript.isMovementFrozen = false;
            Debug.Log("Movement restored! Go explore the crash site.");
        }
    }
}
