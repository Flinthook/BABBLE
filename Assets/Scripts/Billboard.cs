using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{


private Vector3 originalRotation;

    [Header("Lock Rotation")]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;



    [SerializeField]
    private BillboardType billboardType;
    public enum BillboardType { LookAtCamera, CameraForward };

    

    void Awake()
    {
        originalRotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        switch (billboardType)
        {
            case BillboardType.LookAtCamera:
            transform.LookAt(Camera.main.transform.position, Vector3.up);
            break;
            case BillboardType.CameraForward:
            transform.forward = Camera.main.transform.forward;
                break;
            default:
                break;

        }


        Vector3 rotation = transform.rotation.eulerAngles;
        if (lockX)
        {
            rotation.x = originalRotation.x;
        }
        if (lockY)
        {
            rotation.y = originalRotation.y;
        }
        if (lockZ)
        {
            rotation.z = originalRotation.z;
        }
        transform.rotation = Quaternion.Euler(rotation);

    }
}