using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CassetteMenuManager : MonoBehaviour
{
    public GameObject cassettePanel;
    public TMP_Text songInfoText;
    public Image cassetteImage;
    public Button playButton;
    public Button stopButton;
    public Button nextButton;
    public Button prevButton;
    private SoundManager soundManager;
    public MonoBehaviour cameraController; // Assign your camera controller script here

    private int currentIndex = 0;
    private bool menuOpen = false;

    void Start()
    {
        soundManager = SoundManager.Instance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            menuOpen = !menuOpen;
            cassettePanel.SetActive(menuOpen);

            if (menuOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (cameraController != null)
                    cameraController.enabled = false; // Lock camera
                ShowCurrentSong();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (cameraController != null)
                    cameraController.enabled = true; // Unlock camera
            }
        }
    }

    void ShowCurrentSong()
    {
        if (soundManager == null || soundManager.soundClips == null || soundManager.soundClipInfo == null)
        {
            Debug.LogError("SoundManager or its lists are not assigned!");
            return;
        }

        if (soundManager.soundClips.Count == 0)
        {
            if (songInfoText != null)
                songInfoText.text = "No songs collected";
            if (cassetteImage != null)
                cassetteImage.enabled = false;
            playButton.interactable = false;
            stopButton.interactable = false;
            nextButton.interactable = false;
            prevButton.interactable = false;
            return;
        }

        if (songInfoText != null)
            songInfoText.text = soundManager.soundClipInfo[currentIndex];

        // Show cassette image if available
        if (cassetteImage != null && soundManager.cassetteImages != null && soundManager.cassetteImages.Count > currentIndex && soundManager.cassetteImages[currentIndex] != null)
        {
            cassetteImage.sprite = soundManager.cassetteImages[currentIndex];
            cassetteImage.enabled = true;
        }
        else if (cassetteImage != null)
        {
            cassetteImage.enabled = false;
        }

        playButton.interactable = true;
        stopButton.interactable = true;
        nextButton.interactable = soundManager.soundClips.Count > 1;
        prevButton.interactable = soundManager.soundClips.Count > 1;
    }

    public void PlayCurrentSong()
    {
        soundManager.PlaySound(soundManager.soundClips[currentIndex]);
        cassettePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        menuOpen = false;
        if (cameraController != null)
            cameraController.enabled = true; // Unlock camera when closing menu
    }

    public void StopMusic()
    {
        soundManager.GetComponent<AudioSource>().Stop();
    }

    public void NextSong()
    {
        if (soundManager.soundClips.Count == 0) return;
        currentIndex = (currentIndex + 1) % soundManager.soundClips.Count;
        ShowCurrentSong();
    }

    public void PrevSong()
    {
        if (soundManager.soundClips.Count == 0) return;
        currentIndex = (currentIndex - 1 + soundManager.soundClips.Count) % soundManager.soundClips.Count;
        ShowCurrentSong();
    }
}