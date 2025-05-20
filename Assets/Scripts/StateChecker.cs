using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChecker : MonoBehaviour
{
    public float regenerationTime = 5f; // Time in seconds before a wall regenerates

    // Method to register a destroyed wall
    public void RegisterDestroyedWall(GameObject wall, GameObject brokenPieces)
    {
        if (wall == null)
        {
            Debug.LogWarning("Cannot register a null wall!"); // Debug log for null wall
            return;
        }

        Debug.Log($"Registering wall for regeneration: {wall.name}"); // Debug log
        StartCoroutine(ReenableWall(wall, brokenPieces));
    }

    // Coroutine to re-enable a wall after the specified regeneration time
    private IEnumerator ReenableWall(GameObject wall, GameObject brokenPieces)
    {
        Debug.Log($"Waiting to re-enable wall: {wall.name}"); // Debug log

        // Wait for the specified regeneration time
        yield return new WaitForSeconds(regenerationTime);

        // Re-enable the wall
        if (wall != null)
        {
            Debug.Log($"Re-enabling wall: {wall.name}"); // Debug log
            wall.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Wall reference is null!"); // Debug log for null reference
        }

        // Destroy the broken pieces (PAREDROTA)
        if (brokenPieces != null)
        {
            Debug.Log($"Destroying broken pieces: {brokenPieces.name}"); // Debug log
            Destroy(brokenPieces);
        }
    }
}
