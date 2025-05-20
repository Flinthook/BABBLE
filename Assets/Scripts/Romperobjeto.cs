using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Romperobjeto : MonoBehaviour
{
    public GameObject destroyedObject; // Prefab for the broken pieces
    public float verticalOffset = 0f; // Offset for spawning the broken pieces
    public StateChecker stateChecker; // Reference to the StateChecker

    public void DestruirYRemplazar()
    {
        // Adjust the spawn position to include the vertical offset
        Vector3 spawnPosition = transform.position;
        spawnPosition.y += verticalOffset; // Apply the offset

        // Instantiate the broken pieces (this creates a clone in the scene)
        GameObject brokenWallClone = Instantiate(destroyedObject, spawnPosition, Quaternion.identity);

        // Notify the StateChecker to handle regeneration
        if (stateChecker != null)
        {
            stateChecker.RegisterDestroyedWall(this.gameObject, brokenWallClone); // Pass the instantiated clone
        }

        // Disable the original wall
        gameObject.SetActive(false);
    }
}