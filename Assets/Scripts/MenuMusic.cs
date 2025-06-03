using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public AudioClip backgroundMusic; // Assign in Inspector
    [Range(0f, 1f)]
    public float volume = 0.5f;

    private AudioSource audioSource;

    void Awake()
    {
        // Remove persistence so music does not continue between scenes
        // (No DontDestroyOnLoad here)

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        if (backgroundMusic != null && !audioSource.isPlaying)
            audioSource.Play();
    }
}