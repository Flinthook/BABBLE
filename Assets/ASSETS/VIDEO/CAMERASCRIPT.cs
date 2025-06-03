using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CAMERASCRIPT : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private bool videoPlaying = false;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlaying = true;
        }
    }

    void Update()
    {
        if (videoPlaying && Input.GetKeyDown(KeyCode.Escape))
        {
            GoToTutorial();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        GoToTutorial();
    }

    private void GoToTutorial()
    {
        videoPlaying = false;
        SceneManager.LoadScene("SCENE 1 CHURCH");
    }
}
