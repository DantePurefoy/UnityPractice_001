using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonControl : MonoBehaviour
{

    public CharacterController controller;
    public Transform cam;

    [Header("Speed")]
    public float speed;
    public float walkSpeed = 5f;
    public float sprintSpeed = 15f;
    public float momentum;
    public float speedChangeRate = 2f;
    

    private float targetSpeed;

    [Header("Verticality")]
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public bool gravityOn;


    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;


    Vector3 velocity;
    public bool isGrounded;
    public bool isIdle;
    public bool isWalking;
    public bool isSprinting;
    public bool isAirborne;
    public bool isWallRunning;
    public bool isWallClimbing;
    public bool isLedgeClimbing;
   

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;


    //stores the current state
    public MovementState state;

    public enum MovementState
    {
        idle,
        walking,
        sprinting,
        wallrunning,
        wallclimbing,
        ledgeclimbing,
        airborne,

    }


    // Update is called once per frame
    void Update()
    {

        GroundCheck();
        StateHandler();
        Movement();
   

    }

    public void GroundCheck()
    {
        // groundcheck
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -10f;
        }
    }

    public void Movement()
    {
        //basic movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;


        //DOESN'T SEEM TO BE DECCELERATING on no Input
        // NEED float momentumEnd = 0f; if momentum <  maybe 1/4 walk speed?




        //if player isn't moving, set targetSpeed = 0.0f
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            targetSpeed = 0.00f;
            

        }
        // sprinting acceleration, decceleration, and momentum
        momentum = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;

        if (momentum < targetSpeed - speedOffset ||
            momentum > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(momentum, targetSpeed, speedChangeRate * Time.deltaTime + .015f)/*.magnitude?*/;

            Debug.Log("Accelerating or Decelerating");

        }
        else
        {
            speed = targetSpeed;

        }


        //jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }


        //gravity

        if (gravityOn == true)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        
        //Smoothing out Rotations
        if (direction.magnitude >= 0.1f)
        {
                //this is where we get the direction the camera is facing
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

            if (!isWallRunning || !isWallClimbing)
            {
                //character turns with camera
                // how can we turn this off during Acrobatics?
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime); //Time.deltaTime makes movement framerate independent
            }
        }
    }

    public void StateHandler()
    {
        // State = Sprinting
        if (isGrounded && Input.GetKey(KeyCode.LeftShift))
        {
            state = MovementState.sprinting;
           
            isSprinting = true;
            isIdle = false;
            isWalking = false;
            isAirborne = false;
        }

        // State = Walking
        else if (isGrounded)
        {
            state = MovementState.walking;
           
            isWalking = true;
            isIdle = false;
            isSprinting = false;
            isAirborne = false;
        }

        // State = Idle
        else if (isGrounded && targetSpeed == 0)
        {
            state = MovementState.idle;

            isIdle = true;
            isWalking = false;
            isSprinting = false;
            isAirborne = false;
        }

        //State = Airborne
        else
        {
            state = MovementState.airborne;
          
            isAirborne = true;
            isIdle = false;
            isSprinting = false;
            isWalking = false;
        }

        //State = Wallrunning
        if (isWallRunning)
        {
            state = MovementState.wallrunning;
            
            //gravityOn = false;
            isAirborne = false;
            isWallClimbing = false;
            isSprinting = false;
            isWalking = false;
        }
        else
        {
            gravityOn = true;

        }

        //State = WallClimbing
        if (isWallClimbing)
        {
            state = MovementState.wallclimbing;

            
            //gravityOn = false;
            isAirborne = false;
            isWallRunning = false;
            isSprinting = false;
            isWalking = false;
        }
        //else
        //{
        //    gravityOn = true;
            
        //}
    }
}
