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
    public TMPro.TextMeshProUGUI warningText;

    // New: Particle system and multiple meshes to activate
    public GameObject rainParticleSystemGroup; // Assign the empty parent GameObject in Inspector
    public GameObject[] meshesToActivate;     // Assign all meshes in Inspector
    public Transform newRespawnPoint;         // Assign new respawn point in Inspector
    public GameObject[] meshesToDestroy; // Assign all meshes to destroy in Inspector

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

        // Do something each time a pillar is destroyed
        // For example, play a sound, update UI, etc.

        if (destroyedPillars == 2)
        {
            OnTwoPillarsDestroyed();
        }

        if (destroyedPillars >= totalPillars)
        {
            AllPillarsDestroyed();
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

        // 5. Show warning text for a few seconds
        if (warningText != null)
        {
            warningText.gameObject.SetActive(true);
            StartCoroutine(HideWarningTextAfterSeconds(3f));
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

    private IEnumerator HideWarningTextAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (warningText != null)
            warningText.gameObject.SetActive(false);
    }

    private void AllPillarsDestroyed()
    {
        Debug.Log("All pillars destroyed! Loading scene: SCENE 4 POLILLA");

        // Increase gravity until the player is grounded again
        if (player != null)
            player.IncreaseGravityUntilGrounded();

        SceneManager.LoadScene("SCENE 4 POLILLA");
    }

    void Update()
    {
        // For testing: Press J to destroy two pillars instantly
        if (Input.GetKeyDown(KeyCode.J))
        {
            // Only trigger if not already at or above 2
            if (destroyedPillars < 2)
            {
                destroyedPillars = 2;
                OnTwoPillarsDestroyed();
            }
        }
    }

    // Add this coroutine to your class:
    private IEnumerator PlayWarningSoundRepeatedly(int times, float interval)
    {
        for (int i = 0; i < times; i++)
        {
            audioSource.PlayOneShot(warningClip);
            yield return new WaitForSeconds(interval);
        }
    }
}