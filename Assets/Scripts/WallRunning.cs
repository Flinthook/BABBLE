using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallRunning : MonoBehaviour
{

    [Header("Wall Running")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;

    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float maxWallRunTime;

    private float wallRunTimer;

    public float wallClimbSpeed;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;

    private bool upwardsRunning;
    private bool downwardsRunning;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;

    private bool wallRight;
    private bool wallLeft;

    [Header("Exiting")]

    private bool exitingWall;
    public float exitWallTime;

    private float exitWallTimer;

    private Vector3 lastWallNormal; //la ultima normal de la pared


    [Header("References")]
    public Transform orientation;
    private PlayerMovement pm;
    private Rigidbody rb;

    public PlayerCam cam;


    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForWall();
        StateMachine();

    }

    void FixedUpdate()
    {

        if (pm.wallrunning)
            WallRunningMovement();


    }


    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        //Getting Inputs
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        //State 1 - Wall Running
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.wallrunning)
                StartWallRun();

            //wallrun timer
            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;

            if (wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
            }


            //wall jump
            if (Input.GetKeyDown(jumpKey)) WallJump();
        }
        // State 2 - Exiting
        else if (exitingWall)
        {
            if (pm.wallrunning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;



        }
        // State 3 - None
        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }



        if (pm.grounded)
        {
            exitWallTimer = exitWallTime;
            exitingWall = false;
        }

    }

    private void StartWallRun()
    {
        pm.wallrunning = true;
        wallRunTimer = maxWallRunTime;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset the Y velocity to 0


        //apply camera effects
        cam.DoFov(90f);
        if (wallLeft) cam.DoTilt(-5f);
        if (wallRight) cam.DoTilt(5f);

    }

    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }
        //forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);


        //upward/downward force
        if (upwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z); // Climb up the wall
        if (downwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        //push to wall force
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            rb.AddForce(-wallNormal * 100, ForceMode.Force);


        if (useGravity)
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
        rb.useGravity = true; // Re-enable gravity when not wall running
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset the Y velocity to 0

        /*// Reset the last wall normal when wall running stops
         lastWallNormal = Vector3.zero; */

        //Reset camera effects
        cam.DoFov(80f);
        cam.DoTilt(0f);



    }

    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        // Determine the current wall's normal
        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        // Force the player to jump away from the wall
        Vector3 jumpDirection = wallRight ? -orientation.right : orientation.right;

        // Calculate the force to apply for the wall jump
        Vector3 forceToApply = transform.up * wallJumpUpForce + jumpDirection * wallJumpSideForce;

        // Reset velocity and apply the jump force
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset the Y velocity to 0 before jumping
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
