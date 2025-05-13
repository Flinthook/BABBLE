using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{

    [Header("Bash Check")]
    public LayerMask whatIsBreakable;
    public float playerHeight;
    public bool withinBashDistance;
    public float bashDistance = 1f;

    public float bashUp = 1f; // Upward force applied when destroying breakable objects
    public float bashBackwards = 1f; // Backward force applied when destroying breakable objects

    public float groundHitUp = 1f; // Upward force applied when looking at the ground and pressing the mouse button

    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    private Rigidbody rb;
    private PlayerMovement pm;


    [Header("Dashing")]
    public float dashForce;
    public float dashDuration;
    public float dashUpwardForce;

    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    [Header("CameraEffects")]
    public PlayerCam cam;
    public float dashFov;


    [Header("Settings")]
    public bool useCameraForward;
    public bool allowAllDirections;
    public bool disableGravity;
    public bool resetVel;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.LeftShift;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
   void Update()
{
    if (Input.GetKeyDown(dashKey))
        Dash();

    if (dashCdTimer > 0)
        dashCdTimer -= Time.deltaTime;

    if (pm.grounded)
        dashCdTimer = 0;

    // Update withinBashDistance based on raycast in the player's forward direction
    withinBashDistance = Physics.Raycast(transform.position, orientation.forward, bashDistance, whatIsBreakable);

    // Check for mouse click to destroy breakable objects
    if (Input.GetMouseButtonDown(0)) // Left mouse button
    {
        DestroyBreakableObject();

        // Launch the player upwards if grounded and looking at the ground
        if (pm.grounded)
        {
            LaunchUpwards();
        }
    }
}


    private void Dash()
    {

        if (dashCdTimer > 0) return;
        else dashCdTimer = dashCd;

        pm.dashing = true;

        cam.DoFov(dashFov);

        Transform forwardT;
        if (useCameraForward) forwardT = playerCam;
        else forwardT = orientation;

        Vector3 direction = GetDirection(forwardT);


        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
            rb.useGravity = false;
        else
            rb.useGravity = true;

        rb.AddForce(forceToApply, ForceMode.Impulse);

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;

    private void DelayedDashForce()
    {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
        if (resetVel)
            rb.velocity = Vector3.zero;

    }

    private void ResetDash()
    {
        pm.dashing = false;
        rb.useGravity = true;
        cam.DoFov(85f);
    }


    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirections)

            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;

        else
            direction = forwardT.forward * verticalInput;

        if (verticalInput == 0 && horizontalInput == 0)
            direction = forwardT.forward;

        return direction.normalized;

    }

  private void DestroyBreakableObject()
{
    // Check if the player is within bash distance
    if (!withinBashDistance) return;

    // Cast a ray from the camera to the mouse position
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    // Check if the ray hits an object within the whatIsBreakable layer
    if (Physics.Raycast(ray, out hit, Mathf.Infinity, whatIsBreakable))
    {
        // Destroy the object if it is in the whatIsBreakable layer
        Debug.Log($"Destroyed: {hit.collider.gameObject.name}");
        GameObject destroyedObject = hit.collider.gameObject;
        Destroy(destroyedObject);

        // Call the custom function
        OnBreakableObjectDestroyed(destroyedObject);
    }
}

private void OnBreakableObjectDestroyed(GameObject destroyedObject)
{
    // Reset the dash cooldown
    dashCdTimer = 0;
    if (!pm.grounded) {
    // Apply an upward force to the player

   rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset the Y velocity to 0 before jumping

    Vector3 upwardForce = new Vector3(0, bashUp, 0);

    // Apply a backward force relative to the player's orientation
    Vector3 backwardForce = -orientation.forward * bashBackwards;

    // Combine the upward and backward forces
    Vector3 combinedForce = upwardForce + backwardForce;

    // Apply the combined force to the player's Rigidbody
    rb.AddForce(combinedForce, ForceMode.Impulse);

    Debug.Log($"Player launched upwards and backwards after destroying: {destroyedObject.name}");
    }
}

private void LaunchUpwards()
{
    // Check if the player is looking at the ground
    if (playerCam.forward.y > -0.5f) // Adjust the threshold (-0.5f) as needed
    {
        Debug.Log("Player is not looking at the ground. No upward force applied.");
        return;
    }

    // Apply an upward force to the player's Rigidbody
    Vector3 upwardForce = new Vector3(0, groundHitUp, 0); // Use the bashUp value for the force
    rb.AddForce(upwardForce, ForceMode.Impulse);

    Debug.Log("Player launched upwards!");
}
}
