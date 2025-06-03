using UnityEngine;
using UnityEngine.UI;

public class GlobalVolumeSlider : MonoBehaviour
{
    public Slider volumeSlider; // Assign in Inspector

    void Start()
    {
        // Load saved volume or set to 1 (max) by default
        float savedVolume = PlayerPrefs.GetFloat("GlobalVolume", 1f);
        AudioListener.volume = savedVolume;
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("GlobalVolume", value);
        PlayerPrefs.Save();
    }
}