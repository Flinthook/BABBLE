using UnityEngine;
using TMPro;

public class HintItem : MonoBehaviour
{
    [TextArea]
    public string hintText; // The hint to display

    public float displayDistance = 4f; // (Unused now, but you can remove if you want)
    public Canvas hintCanvas;           // Assign a world-space Canvas in Inspector
    public TMP_Text hintTextUI;         // Assign a TMP_Text on the Canvas

    private Camera mainCam;

    void Start()
    {
        if (hintCanvas != null)
            hintCanvas.gameObject.SetActive(true); // Always visible

        mainCam = Camera.main;
        if (hintTextUI != null)
            hintTextUI.text = hintText;
    }

    void Update()
    {
        // No longer rotates to face the player
    }
}