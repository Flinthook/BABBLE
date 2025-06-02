using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SceneIntroVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Assign in Inspector
    public PlayerMovement player;    // Assign in Inspector

    // Unique key for each scene to track if video was played
    private string GetSceneKey() => "IntroPlayed_" + SceneManager.GetActiveScene().name;

    void Start()
    {
        // Check if the intro video for this scene has already played
        if (PlayerPrefs.GetInt(GetSceneKey(), 0) == 0 && videoPlayer != null && player != null)
        {
            // Disable player movement
            player.enabled = false;

            // Play the video
            videoPlayer.gameObject.SetActive(true);
            videoPlayer.Play();

            // Subscribe to video end event
            videoPlayer.loopPointReached += OnVideoFinished;
        }
        else
        {
            // If already played, ensure player can move and video is hidden
            if (player != null) player.enabled = true;
            if (videoPlayer != null) videoPlayer.gameObject.SetActive(false);
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // Mark as played
        PlayerPrefs.SetInt(GetSceneKey(), 1);
        PlayerPrefs.Save();

        // Enable player movement
        if (player != null) player.enabled = true;

        // Hide video
        if (videoPlayer != null) videoPlayer.gameObject.SetActive(false);

        // Unsubscribe
        videoPlayer.loopPointReached -= OnVideoFinished;
    }
}