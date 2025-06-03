using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MOVINGSCRIPT : MonoBehaviour
{
    public string sceneToLoad;                // Name of the scene to load
    public RawImage transitionRawImage;       // Assign a UI RawImage in the Inspector (Canvas, set inactive by default)
    public AudioClip transitionSound;         // Assign the audio clip in the Inspector
    public float transitionDuration = 2f;     // Duration to show image/audio before scene loads
    public bool quitAfterTransition = false;  // If true, quits the game after transition

    private AudioSource audioSource;
    private bool transitioning = false;

    void Start()
    {
        // Optionally, add an AudioSource if not present
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!transitioning && other.CompareTag("Player"))
        {
            StartCoroutine(PlayTransitionAndLoad());
        }
    }

    private IEnumerator PlayTransitionAndLoad()
    {
        transitioning = true;

        // Show RawImage
        if (transitionRawImage != null)
            transitionRawImage.gameObject.SetActive(true);

        // Play sound
        if (transitionSound != null && audioSource != null)
            audioSource.PlayOneShot(transitionSound);

        // Wait for the duration
        yield return new WaitForSeconds(transitionDuration);

        if (quitAfterTransition)
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
