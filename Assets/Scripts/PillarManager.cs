using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PillarManager : MonoBehaviour
{
    public static PillarManager Instance { get; private set; }
    public int totalPillars = 3;
    private int destroyedPillars = 0;

    // References for player, audio, and UI elements
    public PlayerMovement player;
    public Transform teleportTarget;
    public AudioSource audioSource;
    public AudioClip warningClip;
    public RawImage warningImage; // Assign your warning RawImage in the Inspector
    public RawImage finalWarningImage; // Assign a RawImage for the last pillar warning
    public AudioClip finalWarningClip; // Assign an audio clip for the last pillar warning

    public GameObject rainParticleSystemGroup;
    public GameObject[] meshesToActivate;
    public Transform newRespawnPoint;
    public GameObject[] meshesToDestroy;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Call this when a pillar is destroyed
    public void PillarDestroyed()
    {
        destroyedPillars++;
        Debug.Log($"Pillar destroyed! {destroyedPillars}/{totalPillars}");

        if (destroyedPillars == 2)
        {
            OnTwoPillarsDestroyed();
        }

        if (destroyedPillars >= totalPillars)
        {
            StartCoroutine(ShowFinalWarningAndLoadScene());
        }
    }

    private void OnTwoPillarsDestroyed()
    {
        Debug.Log("Two pillars destroyed! Trigger something special here.");

        // 1. Deactivate the death fall timer
        if (player != null)
            player.maxFallTime = Mathf.Infinity;

        // 2. Move the respawn point
        if (player != null && newRespawnPoint != null)
            player.checkpoint = newRespawnPoint;

        // 3. Kill the player (they will respawn at the new checkpoint)
        if (player != null)
            player.Die();

        // 4. Play a sound multiple times
        if (audioSource != null && warningClip != null)
            StartCoroutine(PlayWarningSoundRepeatedly(3, 0.7f)); // Play 3 times, 0.7s apart

        // 5. Show warning image for a few seconds
        if (warningImage != null)
        {
            warningImage.gameObject.SetActive(true);
            StartCoroutine(HideWarningImageAfterSeconds(3f));
        }

        // 6. Activate particle system group
        if (rainParticleSystemGroup != null)
        {
            foreach (var ps in rainParticleSystemGroup.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }
        }

        // 7. Activate all meshes
        if (meshesToActivate != null)
        {
            foreach (var mesh in meshesToActivate)
            {
                if (mesh != null)
                    mesh.SetActive(true);
            }
        }

        // 8. Destroy specified meshes
        if (meshesToDestroy != null)
        {
            foreach (var mesh in meshesToDestroy)
            {
                if (mesh != null)
                    Destroy(mesh);
            }
        }

        // 9. Increase gravity until the player is grounded again
        if (player != null)
            player.IncreaseGravityUntilGrounded();
    }

    private IEnumerator ShowFinalWarningAndLoadScene()
    {
        Debug.Log("All pillars destroyed! Showing final warning and loading scene: SCENE 4 POLILLA");

        // Show final warning image
        if (finalWarningImage != null)
            finalWarningImage.gameObject.SetActive(true);

        // Play final warning sound
        if (audioSource != null && finalWarningClip != null)
            audioSource.PlayOneShot(finalWarningClip);

        // Wait for the duration of the sound or 2 seconds if no sound
        float waitTime = (finalWarningClip != null) ? finalWarningClip.length : 2f;
        yield return new WaitForSeconds(waitTime);

        // Hide the image
        if (finalWarningImage != null)
            finalWarningImage.gameObject.SetActive(false);

        // Increase gravity until the player is grounded again
        if (player != null)
            player.IncreaseGravityUntilGrounded();

        // Load the next scene
        SceneManager.LoadScene("SCENE 4 POLILLA");
    }

    private IEnumerator HideWarningImageAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (warningImage != null)
            warningImage.gameObject.SetActive(false);
    }

    private IEnumerator PlayWarningSoundRepeatedly(int times, float interval)
    {
        for (int i = 0; i < times; i++)
        {
            audioSource.PlayOneShot(warningClip);
            yield return new WaitForSeconds(interval);
        }
    }

    void Update()
    {
        // For testing: Press J to destroy two pillars instantly
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (destroyedPillars < 2)
            {
                destroyedPillars = 2;
                OnTwoPillarsDestroyed();
            }
        }
    }
}