using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{

[Header("Wall Running")]
public LayerMask whatIsWall;
public LayerMask whatIsGround;
public float wallRunForce;
public float maxWallRunTime;
    
    public float wallJumpUpForce;
public float wallJumpSideForce;

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


[Header("References")]
public Transform orientation;
private PlayerMovement pm;
private Rigidbody rb;


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

    void FixedUpdate(){

    if (pm.wallrunning)
        WallRunningMovement();

    
    }


    private void CheckForWall(){
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround(){
        return Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine(){
        //Getting Inputs
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        //State 1 - Wall Running
        if ((wallLeft || wallRight ) && verticalInput > 0 && AboveGround()  && !exitingWall)
        {
           if (!pm.wallrunning)
           StartWallRun();
        }

        if (Input.GetKeyDown(jumpKey) && pm.wallrunning)
        {
            WallJump();
        }

        else if(exitingWall) 
        {
            if (pm.wallrunning)
                StopWallRun();

                if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

                if (exitWallTimer <= 0)
                
                    exitingWall = false;                
        }

        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;


    }

    private void WallRunningMovement()
    {
rb.useGravity = false; // Disable gravity while wall running
rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset the Y velocity to 0

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude){
            wallForward = -wallForward;
        }
        //forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);


        //upward/downward force
        if(upwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z); // Climb up the wall
        if(downwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        //push to wall force
        if(!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
                rb.AddForce(-wallNormal * 100, ForceMode.Force);

    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
        rb.useGravity = true; // Re-enable gravity when not wall running
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset the Y velocity to 0
    }

    private void WallJump()
    {
            //enter exiting wall state
            exitingWall = true;
            exitWallTimer = exitWallTime;

        Vector3  wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 forceToApply = wallNormal * wallJumpSideForce + transform.up * wallJumpUpForce;

        //add force and reset velocity
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset the Y velocity to 0 before jumping
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
