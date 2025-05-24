using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIActionImage : MonoBehaviour
{
    public Image targetImage;
    public Sprite clickSprite;
    public Sprite mainSprite;
    public float displayTime = 0.5f;

    public float clickScale = 1.2f; // Scale factor for click sprite
    public float scaleLerpSpeed = 10f; // How fast to return to normal scale

    public float bobAmount = 10f;
    public float bobSpeed = 8f;
    public float bobSwayAmount = 5f;
    public float airBobMultiplier = 0.5f;

    private Vector2 originalPos;
    private float bobTimer = 0f;
    private Vector3 originalScale;

    public PlayerMovement playerMovement;

    void Start()
    {
        if (targetImage)
        {
            originalPos = targetImage.rectTransform.anchoredPosition;
            originalScale = targetImage.rectTransform.localScale * clickScale; // Start bigger
            targetImage.rectTransform.localScale = originalScale; // Apply starting scale
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShowClick();
        }

        // FPS-style head bobbing: only when moving
        bool isMoving = true;
        bool isAir = false;
        bool isDashing = false;
        if (playerMovement != null)
        {
            isMoving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f;
            isAir = !playerMovement.grounded;
            isDashing = playerMovement.dashing;
        }

        if (targetImage)
        {
            if (isMoving)
            {
                bobTimer += Time.deltaTime * bobSpeed;
                float bobMod = isAir ? airBobMultiplier : 1f;
                float yBob = Mathf.Abs(Mathf.Sin(bobTimer)) * bobAmount * bobMod;
                float xBob = Mathf.Sin(bobTimer * 0.5f) * bobSwayAmount * bobMod;
                targetImage.rectTransform.anchoredPosition = originalPos + new Vector2(xBob, yBob);
            }
            else
            {
                bobTimer = 0f;
                targetImage.rectTransform.anchoredPosition = Vector2.Lerp(targetImage.rectTransform.anchoredPosition, originalPos, Time.deltaTime * 10f);
            }

            // --- Make image bigger when dashing ---
            float dashScale = isDashing ? clickScale : 1f;
            targetImage.rectTransform.localScale = Vector3.Lerp(targetImage.rectTransform.localScale, originalScale * dashScale, Time.deltaTime * scaleLerpSpeed);
        }
    }

    public void ShowClick()
    {
        if (targetImage && clickSprite)
        {
            targetImage.sprite = clickSprite;
            targetImage.rectTransform.localScale = originalScale * clickScale;
            StopAllCoroutines();
            StartCoroutine(ResetImageAfterDelay());
        }
    }

    private IEnumerator ResetImageAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        if (targetImage && mainSprite)
            targetImage.sprite = mainSprite;
    }
}