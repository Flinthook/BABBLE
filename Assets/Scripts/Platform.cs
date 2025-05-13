using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float fallDistance = 0.5f; // Distance the platform moves down
    public float resetSpeed = 10f;   // Speed at which the platform resets to its original position
    public float minFallSpeed = 1f;  // Minimum speed for the platform to fall smoothly
    public float launchForceMultiplier = 10f; // Multiplier for the upward launch force (increase this value)

    private Vector3 originalPosition;
    private bool isFalling = false;
    private bool playerOnPlatform = false; // Tracks if the player is on the platform

    private void Start()
    {
        // Store the original position of the platform
        originalPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            playerOnPlatform = true; // Player is now on the platform
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRigidbody != null)
            {
                // Use the player's fall speed or a minimum fall speed
                float playerFallSpeed = Mathf.Max(Mathf.Abs(playerRigidbody.velocity.y), minFallSpeed);
                StartCoroutine(FallAndReset(playerRigidbody, collision.gameObject, playerFallSpeed));
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = false; // Player has left the platform
        }
    }

    private IEnumerator FallAndReset(Rigidbody playerRigidbody, GameObject player, float fallSpeed)
    {
        isFalling = true;

        // Move the platform down smoothly based on the fall speed
        Vector3 targetPosition = originalPosition + Vector3.down * fallDistance;
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            Vector3 platformMovement = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime) - transform.position;
            transform.position += platformMovement;

            // Apply the same movement to the player to keep them on the platform
            if (playerOnPlatform)
            {
                playerRigidbody.MovePosition(playerRigidbody.position + platformMovement);
            }

            yield return null;
        }

        // Wait for a moment before resetting
        yield return new WaitForSeconds(0.5f);

        // Slowly reset the platform to its original position
        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            Vector3 platformMovement = Vector3.MoveTowards(transform.position, originalPosition, resetSpeed * Time.deltaTime) - transform.position;
            transform.position += platformMovement;

            // Apply the same movement to the player to keep them on the platform
            if (playerOnPlatform)
            {
                playerRigidbody.MovePosition(playerRigidbody.position + platformMovement);
            }

            yield return null;
        }

        // Launch the player upwards if they are still on the platform
        if (playerOnPlatform)
        {
            Vector3 launchForce = Vector3.up * launchForceMultiplier;
            playerRigidbody.AddForce(launchForce, ForceMode.Impulse);
        }

        // Ensure the platform is exactly at its original position
        transform.position = originalPosition;
        isFalling = false;
    }
}