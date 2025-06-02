using UnityEngine;

public class KillZone : MonoBehaviour
{
    public LayerMask whatIsDeath; // Assign in Inspector

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object's layer is in whatIsDeath
        if (((1 << other.gameObject.layer) & whatIsDeath) != 0)
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.Die();
            }
        }
        else
        {
            // Fallback: kill if it's the player anyway (optional)
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.Die();
            }
        }
    }
}