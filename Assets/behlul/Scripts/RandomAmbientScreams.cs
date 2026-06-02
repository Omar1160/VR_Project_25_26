using UnityEngine;
using System.Collections;

public class RandomAmbientScreams : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] screamClips;

    [Header("Random interval (seconds)")]
    public float minDelay = 10f;
    public float maxDelay = 30f;

    [Header("Volume randomization (optional)")]
    [Range(0f, 1f)] public float minVolume = 0.6f;
    [Range(0f, 1f)] public float maxVolume = 1.0f;

    [Header("Pitch randomization (optional)")]
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    Coroutine loop;

    void Awake()
    {
        if (!source) source = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        loop = StartCoroutine(Loop());
    }

    void OnDisable()
    {
        if (loop != null) StopCoroutine(loop);
        loop = null;
    }

    IEnumerator Loop()
    {
        while (true)
        {
            if (screamClips == null || screamClips.Length == 0 || source == null)
            {
                yield return null;
                continue;
            }

            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            var clip = screamClips[Random.Range(0, screamClips.Length)];
            source.pitch = Random.Range(minPitch, maxPitch);
            source.volume = Random.Range(minVolume, maxVolume);

            source.PlayOneShot(clip);
        }
    }
}
