using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pilares : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyPillar()
    {
        // Notify the manager
        if (PillarManager.Instance != null)
            PillarManager.Instance.PillarDestroyed();

        // Destroy this pillar object
        Destroy(gameObject);
    }
}
