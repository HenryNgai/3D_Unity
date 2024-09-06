using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public AudioSource audioSource;

    void OnEnable()
    {
        // Subscribe to the OnFootstep and OnStopFootstep events
        PlayerMovement.OnFootstep += PlayFootstep;
        PlayerMovement.OnStopFootstep += StopFootstep;
    }

    void OnDisable()
    {
        // Unsubscribe from events to prevent memory leaks
        PlayerMovement.OnFootstep -= PlayFootstep;
        PlayerMovement.OnStopFootstep -= StopFootstep;
    }

    void PlayFootstep()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();  // Play a single footstep sound
        }
    }

    public void StopFootstep()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();  // Stop the footstep sound
        }
    }
}
