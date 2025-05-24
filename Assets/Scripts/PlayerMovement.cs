using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Added for UI elements

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;

    public float walkSpeed;
    public float sprintSpeed;
    public Transform orientation;

    public float wallrunSpeed;
    public float dashSpeed;
    public float dashSpeedChangeFactor;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float fallMultiplier;
    public float jumpFallMultiplier;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public float groundDrag;
    public float airDrag;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("UI Warning")]
    public Image screenOverlay; // Assign a UI Image covering the screen in the Canvas
    public Color warningColor = new Color(1, 0, 0, 0.3f); // Semi-transparent red
    public float warningFadeSpeed = 5f; // How fast to fade in/out

    private bool warningActive = false;

    bool readyToJump;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        air,
        dashing,
        wallrunning,
        dead // Added dead state
    }

    public bool dashing;
    public bool wallrunning;

    // --- Death & Checkpoint ---
    [Header("Death & Checkpoint")]
    public Transform checkpoint; // Assign in inspector or via script
    public float deathYThreshold = -20f; // Y position below which player dies
    public float maxFallTime = 2.5f; // Max time allowed to fall before dying

    private float fallTimer = 0f;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent the rigidbody from rotating
        readyToJump = true;
    }

    void Update()
    {
        if (isDead) return; // Prevent input/movement when dead

        // --- Add this block to respawn on R key press ---
        if (Input.GetKeyDown(KeyCode.R))
        {
            Die();
            return;
        }
        // --- End added block ---

        MyInput(); // Get input from the player

        // Check if the player is grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, (playerHeight) + 0.3f, whatIsGround);

        if (grounded && !dashing)
        {
            rb.drag = groundDrag; // Apply drag when grounded
        }
        else
        {
            rb.drag = airDrag; // Apply air drag when in the air
        }

        SpeedControl(); // Control the speed of the player

        // Apply additional gravity if the spacebar is released while in the air
        if (!grounded && Input.GetKeyUp(jumpKey))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (jumpFallMultiplier - 1) * Time.deltaTime;
        }

        // Apply fall multiplier when falling
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        // --- Death Logic ---
        if (transform.position.y < deathYThreshold)
        {
            Die();
        }

        if (!grounded && rb.velocity.y < 0)
        {
            fallTimer += Time.deltaTime;
            // Show warning 2 seconds before death
            if (maxFallTime - fallTimer <= 2f && fallTimer < maxFallTime)
            {
                warningActive = true;
            }
            else
            {
                warningActive = false;
            }
            if (fallTimer > maxFallTime)
            {
                Die();
            }
        }
        else
        {
            fallTimer = 0f;
            warningActive = false;
        }

        // Handle the overlay fade
        if (screenOverlay)
        {
            Color target = warningActive ? warningColor : new Color(1, 0, 0, 0);
            screenOverlay.color = Color.Lerp(screenOverlay.color, target, Time.deltaTime * warningFadeSpeed);
        }

        StateHandler(); // Handle the player's state (walking, sprinting, air)
    }

    void FixedUpdate()
    {
        if (!isDead)
            MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // A and D keys
        verticalInput = Input.GetAxis("Vertical"); // W and S keys
        if (Input.GetKey(jumpKey) && readyToJump && grounded) // Check if the jump key is pressed and the player is ready to jump
        {
            readyToJump = false; // Set the player to not ready to jump
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown); // Reset the jump cooldown after a delay
        }
    }
    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (horizontalInput != 0 || verticalInput != 0)
        {
            // Apply force to the rigidbody for movement
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            // Reduce velocity to stop sliding
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.velocity = Vector3.Lerp(flatVelocity, Vector3.zero, Time.deltaTime * 10f) + new Vector3(0f, rb.velocity.y, 0f);
        }
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Get the horizontal velocity
        if (flatVel.magnitude > moveSpeed) // If the player is moving faster than the max speed
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed; // Limit the velocity to the max speed
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z); // Apply the limited velocity to the rigidbody
        }
    }

    void ResetJump()
    {
        readyToJump = true; // Reset the jump cooldown
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset the vertical velocity to 0 before jumping
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private float desiredMoveSpeed; // Desired movement speed
    private float lastDesiredMoveSpeed; // Last desired movement speed
    private MovementState lastState; // Last movement state
    private bool keepMomentum; // Flag to keep momentum

    private void StateHandler()
    {
        if (isDead)
        {
            state = MovementState.dead;
            moveSpeed = 0f;
            return;
        }

        // Mode - Sprint
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting; // Set the state to sprinting
            desiredMoveSpeed = sprintSpeed; // Set the movement speed to sprint speed  
        }
        // Mode - Walk
        else if (grounded)
        {
            state = MovementState.walking; // Set the state to walking
            desiredMoveSpeed = walkSpeed; // Set the movement speed to walk speed
        }
        // Mode - Air
        else
        {
            state = MovementState.air; // Set the state to air  
            if (desiredMoveSpeed < sprintSpeed)
                desiredMoveSpeed = walkSpeed;
            else
                desiredMoveSpeed = sprintSpeed; // Set the movement speed to sprint speed
        }
        // Mode -Wallruning
        if (wallrunning)
        {
            state = MovementState.wallrunning; // Set the state to wallrunning
            desiredMoveSpeed = wallrunSpeed; // Set the movement speed to wallrun speed    
        }
        // Mode - Dashing
        if (dashing)
        {
            state = MovementState.dashing; // Set the state to dashing
            desiredMoveSpeed = dashSpeed; // Set the movement speed to dash speed    
            speedChangeFactor = dashSpeedChangeFactor; // Set the speed change factor for dashing
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true; // Keep momentum if the last state was dashing
        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines(); // Stop all coroutines to prevent multiple speed changes
                StartCoroutine(SmoothlyLerpMoveSpeed()); // Smoothly change the speed if desired speed has changed
            }
            else
            {
                StopAllCoroutines(); // Stop all coroutines to prevent multiple speed changes
                moveSpeed = desiredMoveSpeed; // Set the movement speed to the desired movement speed
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state; // Store the last state
    }

    private float speedChangeFactor;
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed); // Calculate the difference between desired and current speed
        float startValue = moveSpeed; // Store the starting value of the speed
        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference); // Smoothly interpolate the speed
            time += Time.deltaTime * boostFactor; // Increase the time based on the boost factor
            yield return null; // Wait for the next frame
        }
        moveSpeed = desiredMoveSpeed; // Set the final speed to the desired speed
        speedChangeFactor = 1f; // Reset the speed change factor
        keepMomentum = false; // Reset the momentum flag
    }

    // --- Death & Respawn ---
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        state = MovementState.dead;
        rb.velocity = Vector3.zero;
        // Optionally: play death animation, disable controls, etc.
        Invoke(nameof(Respawn), 0.1f); // Wait 1 second before respawn
    }

    private void Respawn()
    {
        if (checkpoint != null)
        {
            transform.position = checkpoint.position;
            rb.velocity = Vector3.zero;
        }
        isDead = false;
        fallTimer = 0f;
        state = MovementState.air; // Or whatever state is appropriate
    }

    // --- Checkpoint Setter ---
    public void SetCheckpoint(Transform newCheckpoint)
    {
        checkpoint = newCheckpoint;
    }
}

