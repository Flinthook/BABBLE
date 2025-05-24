using UnityEngine;

public class DestruirJakub : MonoBehaviour
{
    public GameObject obstaculoInvisible; // Assign in Inspector

    public void HideObstacle()
    {
        if (obstaculoInvisible != null)
            obstaculoInvisible.SetActive(false);
    }
}