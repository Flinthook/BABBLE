using UnityEngine;
using System.Collections;

public class CassetteUI : MonoBehaviour
{
    public float displayTime = 5f; // Time in seconds to stay visible
    public float zoomAmount = 1.2f; // How much to scale up
    public float zoomDuration = 0.3f; // How fast to zoom
    public float twirlAngle = 30f; // Degrees to rotate
    public float twirlDuration = 0.3f; // How fast to twirl

    private Vector3 originalScale;
    private Quaternion originalRotation;

    private void OnEnable()
    {
        if (originalScale == Vector3.zero)
            originalScale = transform.localScale;
        if (originalRotation == Quaternion.identity)
            originalRotation = transform.localRotation;

        // Reset to original before animating
        transform.localScale = originalScale;
        transform.localRotation = originalRotation;

        StartCoroutine(AnimateAndHide());
    }

    private IEnumerator AnimateAndHide()
    {
        // Zoom and twirl in
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / zoomDuration;
            float scale = Mathf.Lerp(1f, zoomAmount, t);
            float angle = Mathf.Lerp(0f, twirlAngle, t);
            transform.localScale = originalScale * scale;
            transform.localRotation = originalRotation * Quaternion.Euler(0, 0, angle);
            yield return null;
        }

        // Hold for the rest of displayTime
        yield return new WaitForSeconds(displayTime - zoomDuration);

        // Optionally, animate back to original (optional)
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / twirlDuration;
            float scale = Mathf.Lerp(zoomAmount, 1f, t);
            float angle = Mathf.Lerp(twirlAngle, 0f, t);
            transform.localScale = originalScale * scale;
            transform.localRotation = originalRotation * Quaternion.Euler(0, 0, angle);
            yield return null;
        }

        // Hide
        gameObject.SetActive(false);
    }
}
