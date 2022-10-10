using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Wallrunning")]
   
    public LayerMask Wall;
    public LayerMask Ground;
    public float wallRunSpeed;
    public float maxWallRunTime;
    public float wallRunTimer;

    [Header("Wallrun Arc")]
    public bool wallRunAscend;
    public float wRATime;
    public float wRAMaxTime;
    public float wRAClimb;

    public bool wallRunDescend;
    public float wRDTime;
    public float wRDMaxTime;
    public float wRDClimb;



    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;
    
    public float sphereCastRadius;
    

    public float wallStickForce = 100f;

    [Header("References")]
    public Transform orientation;
    public ThirdPersonControl tpc;
    public CharacterController controller;

    public WallClimb wc;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        tpc = GetComponent<ThirdPersonControl>();
        wc = GetComponent<WallClimb>();
        wallRunTimer = maxWallRunTime;
    }

    private void Update()
    {
        CheckForWall();
        WallRunStateMachine();
    }

    private void FixedUpdate()
    {
        if (tpc.isWallRunning)
            WallRunMovement();
    }

    private void CheckForWall()
    {

        
        wallLeft = Physics.SphereCast(transform.position, sphereCastRadius, - orientation.right, out leftWallHit, wallCheckDistance, Wall);

        wallRight = Physics.SphereCast(transform.position,sphereCastRadius, orientation.right, out rightWallHit, wallCheckDistance, Wall);

        //Debugging
        Vector3 left = transform.TransformDirection(Vector3.left) * wallCheckDistance;
        Vector3 right = transform.TransformDirection(Vector3.right) * wallCheckDistance;
        Debug.DrawRay(orientation.position, right, Color.green);
        Debug.DrawRay(orientation.position, left, Color.cyan);
      
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, Ground);
    }

    private void WallRunStateMachine()
    {
        // Getting Inputs
        //horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //State 1 - Enter WallRun (Later Change to ascending)
        if ((wallLeft || wallRight) && verticalInput > 0 && wc.wallCounter > 0 && /*tpc.momentum >= tpc.walkSpeed * 1.8f &&*/ AboveGround())
        
        {
            if (!tpc.isWallRunning)
                StartWallRun();

            //timer
            if (wallRunTimer > 0) wallRunTimer -= Time.deltaTime;
            if (wallRunTimer < 0) StopWallRun();




            Debug.Log("IsWallRunning");
        }

        //State 2 - Enter Descending Wallrun 

        //State 3 - Ending WallRun
        else
        {
            if (tpc.isWallRunning)
                StopWallRun();
            Debug.Log("WallRunning Ended");
        }
    }

    private void StartWallRun()
    {

        
        tpc.isWallRunning = true;
        
        Debug.Log("WallRun Started");
      
    }

    // can't turn camera withough exiting current WallRunMovement 
    private void WallRunMovement()
    {

        

        tpc.gravityOn = false;

        
        Vector3 wallNormal = wallRight? rightWallHit.normal : leftWallHit.normal;

        //It's working... sometimes. It feels like there's some clashing between different movements?
        Vector3 localWallNormal = transform.InverseTransformDirection(wallNormal);

        Vector3 wallForward = Vector3.Cross(localWallNormal, transform.up).normalized;

        //NEW basic WALLRUN movement
        //float horizontal = Input.GetAxisRaw("Horizontal");
        //float vertical = Input.GetAxisRaw("Vertical");

        //Vector3 move = new Vector3(vertical, 0f, 0f).normalized;

        //controller.Move(move.normalized * wallRunSpeed * Time.deltaTime);


        ////standard wallrun movement
        Vector3 move = new Vector3(Input.GetAxisRaw("vertical"), 0f, 0f);

        move = transform.InverseTransformDirection(Vector3.forward);
        controller.Move(move * Time.deltaTime * wallRunSpeed);

        //allows wallrunning in either direction
        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallRunSpeed = -Mathf.Abs(wallRunSpeed);
            Debug.Log("Detection Works!");
        }


        ////A & D Wallrun
        //Vector3 move2 = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f).normalized;
        //controller.Move(move2 * Time.deltaTime * wallRunSpeed);


        ////arced wallrun movement may require separate funtions

        ////ascent
        //Vector3 move = new Vector3(Input.GetAxisRaw("Vertical"), wallRunSpeed * wRAClimb, 0f).normalized;
        //controller.Move(move * Time.deltaTime * wallRunSpeed);

        ////descent
        //Vector3 move = new Vector3(Input.GetAxisRaw("Vertical"), -(wallRunSpeed * wRDClimb), 0f).normalized;
        //controller.Move(move * Time.deltaTime * wallRunSpeed);

        ////timers

        ////ascent
        //if (wRATime > 0) wRATime -= Time.deltaTime;
        //if (wRATime < 0) StopWallRunAscend(), StartWallRunDescend();

        ////descent
        //if (wRDTime > 0) wRDTime -= Time.deltaTime;
        //if (wRDTime < 0) StopWallRun();





        //stick to wall (need to figure out how to force the character agaist the wall durring wallrun)
        //Vector3 wallStick = new Vector3(0f, 0f, wallNormal);

        //if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        //    controller.Move(wallStick * wallStickForce * Time.deltaTime);

    }

    private void StopWallRun()
    {
        tpc.isWallRunning = false;

        wallRunSpeed = Mathf.Abs(wallRunSpeed);

        //counter(works for now but I think I'd like it in StartWallRun() without ending the wall run
        wc.wallCounter--;

        wallRunTimer = maxWallRunTime;

        //tpc.gravityOn = true; leaving this here just in case
    }


}
