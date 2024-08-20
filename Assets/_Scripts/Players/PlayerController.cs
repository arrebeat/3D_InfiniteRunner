using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Dreamteck.Splines;
using MoreMountains.Feedbacks;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D rb { get; private set; } 
    public Animator animator { get; private set; }
    public SplineComputer spline;
    public SplineFollower follower { get; private set;}
    public CursrorController cursor;
    public MMF_Player feedbacks { get; private set; }
    public MMF_Particles walkSmoke { get; private set; }
    public MMF_Particles jumpSmoke { get; private set; }
    public SpriteRenderer[] renderers { get; private set; }
    public SOPlayerData_Teletustra soPlayerData;

    public bool IsFacingRight { get; private set; }
    public bool IsRunning { get; private set; }
    public bool WasRunning { get; private set; }
    
    // Timers
    //public float WalkTime { get; private set; }
    public float WalkSpeed { get; private set; }    
    public float RunTime { get; private set; }
    public float RunSpeed { get; private set; }  
    public float LastPressedWalkTime { get; private set; }
    public float LastOnGroundTime { get; private set; }
    public float LastPressedJumpTime { get; private set; }
    public float LastLeftClickTime { get; private set; }

    public BatController bat;

    private Coroutine teleportCharge;

    // Mouse
    private bool LeftClick;
    
    // Jump
    public bool IsJumping { get; private set; }
    public bool IsFalling { get; private set; }
    public bool isGrounded;
    private bool _canJump;
    private bool _isJumpCut;
    private bool _isJumpFalling;
    private float _horizontalVelocity;
    private float _verticalVelocity;
    private GameObject latestPlatform;

    public float walkMaxSpeed, walkAccelTime, walkDeccelTime, runMaxSpeed, runAccelTime, runDeccelTime, doubleTapToRunTime, jumpSpeed, coyoteTime, jumpInputBufferTime;

    public bool doConserveMomentum = true;
    public bool HasDied;
    
    //public float runAccelAmount;
    //public float runDeccelAmount;

    // Teleport
    public bool HasTeleported { get; private set; }
    private float TeleportChargeTime;
    public float TeleportChargeTimeFactor;
    

    [SerializeField] public float MaxTeleportDistance;

    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize;


    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    private PlayerControls playerControls;

    [Header("Lerp Debug")]
    [Range(0, 10)]
    public float testTime;
    public float testAccelTime;
    public float testDeccelTime;
    [Range(0, 1)]
    public float testNormalizedTime;
    public float testMaxSpeed;
    [Range(0, 10)]
    public float testSpeed;

    private void OnValidate() 
    {
        //testTime = Mathf.Clamp(testTime, 0, testDeccelTime);
        //testNormalizedTime = testTime / testDeccelTime;
        //runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        //runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        //runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        //runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
    }

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        follower = GetComponent<SplineFollower>();
        feedbacks = GetComponent<MMF_Player>();
        walkSmoke = feedbacks.GetFeedbackOfType<MMF_Particles>(searchedLabel:"WalkSmoke");
        jumpSmoke = feedbacks.GetFeedbackOfType<MMF_Particles>(searchedLabel:"JumpSmoke");
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        playerControls = new PlayerControls();
        
        playerControls.InGame.Jump.started += Jump_started;
        playerControls.InGame.Jump.performed += Jump;
        playerControls.InGame.Jump.canceled += Jump_canceled;

        playerControls.InGame.Walk.started += Walk_started;
        playerControls.InGame.Walk.canceled += Walk_canceled;

        playerControls.InGame.Teleport.started += Teleport_started;
        //playerControls.InGame.Teleport.performed += Teleport;
        playerControls.InGame.Teleport.canceled += Teleport_canceled;

        playerControls.InGame.Shoot.started += Shoot_started;
        playerControls.InGame.Shoot.canceled += Shoot_canceled;
    }
    
    private void OnEnable() 
    {
        playerControls.Enable();   
    }

    private void OnDisable() 
    {
        playerControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        IsFacingRight = true;

        
        //TeleportChargeTime = cursor.TeleportDistance;
    }

    // Update is called once per frame
    void Update()
    {
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;
        LastPressedWalkTime -= Time.deltaTime;
        LastLeftClickTime -= Time.deltaTime;
        #endregion

        #region COLLISION CHECKS
        if (!IsJumping)
        {
            //Ground Check
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping)
            {
                LastOnGroundTime = soPlayerData.coyoteTime;
                HasTeleported = false;
                isGrounded = true;
            }

            if (!Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
            {
                IsFalling = true;
                isGrounded = false;
            }
        }        
        #endregion

        #region JUMP CHECKS
        
        // Falling
        //if (rb.velocity.y < 0)
        //{
        //    IsFalling = true;
        //}

        // Jump falling
        if (IsJumping && rb.velocity.y < 0)
        {
            IsJumping = false;
            _isJumpFalling = true;
        }

        // Can't jump if falling
        if (!IsJumping && LastOnGroundTime < 0)
        {
            _canJump = false;
        }

        // Can jump if Grounded
        if (LastOnGroundTime > 0 && !IsJumping )
        {
            _canJump = true;
            _isJumpCut = false;

            if (!IsJumping)
            {
                IsFalling = false;
                _isJumpFalling = false;
            }
        }

        // Jump input buffer
        if (_canJump && LastPressedJumpTime > 0)
        {
            Jump();
        }
        #endregion
        
        // Get horizontal & vertical velocity and update the public variable
        _horizontalVelocity = rb.velocity.x;
        _verticalVelocity = rb.velocity.y;

        // Read the movement value
        float walkInput = playerControls.InGame.Walk.ReadValue<float>();
        
        Vector3 currentPosition = transform.position;
        
        //testSpeed = Mathf.Lerp(0, testMaxSpeed, testNormalizedTime);
        //RunSpeed = Mathf.Lerp(0, runMaxSpeed, testNormalizedTime);

        if (testSpeed == 0)
        {
            WasRunning = false;
        }

        // Check direction to face
        if (walkInput !=0)
        {
            checkDirectionToFace(walkInput > 0);
            animator.SetFloat("isMoving", Mathf.Abs(walkInput));
            //WalkTime += Time.deltaTime;
            
            if (IsRunning)
            {
                animator.SetBool("isRunning", true);
                //WalkTime = 0;

                testNormalizedTime = testTime / soPlayerData.runAccelTime;
                testTime -= Time.deltaTime;
                testTime = Mathf.Clamp(testTime, 0, soPlayerData.runAccelTime);
            
                testSpeed = Mathf.Lerp(soPlayerData.runMaxSpeed, 0, testNormalizedTime);
            }
            // is walking
            else
            {
                testNormalizedTime = testTime / soPlayerData.walkAccelTime;
                testTime -= Time.deltaTime;
                testTime = Mathf.Clamp(testTime, 0, soPlayerData.walkAccelTime);
            
                testSpeed = Mathf.Lerp(soPlayerData.walkMaxSpeed, 0, testNormalizedTime);
            }
        }
        // (walkInput = 0)
        else
        {
            animator.SetFloat("isMoving", 0);
            animator.SetBool("isRunning", false);

            //testNormalizedTime = (testSpeed > testMaxSpeed && testSpeed <= runMaxSpeed) ? testTime /runDeccelTime : testTime / testDeccelTime;
            if (WasRunning)
            {
                testNormalizedTime = testTime / soPlayerData.runDeccelTime;
                testSpeed = Mathf.Lerp(0, soPlayerData.runMaxSpeed, testNormalizedTime);
                
            }
            // Wasn't running
            else
            {
                testNormalizedTime = testTime / soPlayerData.walkDeccelTime;
                testSpeed = Mathf.Lerp(0, soPlayerData.walkMaxSpeed, testNormalizedTime);
                
            }
            
            testTime = Mathf.Clamp(testTime, 0, soPlayerData.runDeccelTime);
            testTime -= Time.deltaTime;

            if (testTime < 0)
            {
                testTime = 0;
            }
        }

        // Move the player     
        
        //WalkSpeed = (walkInput !=0) ? Mathf.Lerp(0, walkMaxSpeed, WalkTime / walkAccelTime) : Mathf.Lerp(0, WalkSpeed, WalkTime / walkDeccelTime);
        //WalkSpeed = testSpeed;        

        if (!IsRunning)
        {
            animator.SetBool("isRunning", false);
            RunTime = 0;

            currentPosition.x += (walkInput != 0) ? walkInput * testSpeed * Time.deltaTime : (IsFacingRight) ? testSpeed * Time.deltaTime : -testSpeed * Time.deltaTime;
            transform.position = currentPosition;

            if (WasRunning)
            {
                if (walkInput != 0)
                {
                    
                }

            }
        }

        if (IsRunning)
        {
            RunTime += Time.deltaTime;
            currentPosition.x += walkInput * testSpeed * Time.deltaTime;
            transform.position = currentPosition;
        }     
        


        TeleportChargeTime = cursor.TeleportDistance * soPlayerData.teleportChargeTimeFactor;
    }

    private void FixedUpdate() 
    {
        if (IsRunning)
        {
            //Run(1);
        }    
    }
  
#region INPUT CALLBACKS
    private void Walk_started(InputAction.CallbackContext context)
    {
        //Debug.Log(context);
        
        //WalkTime = 0;
        
        if (isGrounded)
            walkSmoke.Play(transform.position, 1);

        if (LastPressedWalkTime > 0)
        {
            //walkSpeed = runSpeed;
            IsRunning = true;

            testTime = (testSpeed == 0) ? soPlayerData.runAccelTime : soPlayerData.runAccelTime * (1 - testNormalizedTime);
            //testTime = runAccelTime;
        }
        else
        {
            testTime = (testSpeed == 0) ? soPlayerData.walkAccelTime : soPlayerData.walkAccelTime * (1 - testNormalizedTime);
        }
    }
    private void Walk_canceled(InputAction.CallbackContext context)
    {
        //Debug.Log(context);

        walkSmoke.Stop(transform.position, 1);
 
        if (!IsRunning)
        {
            LastPressedWalkTime = soPlayerData.doubleTapToRunTime;

            testTime = (testSpeed == soPlayerData.walkMaxSpeed) ? soPlayerData.walkDeccelTime : soPlayerData.walkDeccelTime * (1 - testNormalizedTime);
        }

        if (IsRunning)
        {
            LastPressedWalkTime = soPlayerData.doubleTapToRunTime;

            //testTime = (testSpeed == runMaxSpeed) ? runDeccelTime : (testSpeed <= testMaxSpeed) ? testDeccelTime * (1 - testNormalizedTime) : runDeccelTime * (1 - testNormalizedTime);
            if (testSpeed == soPlayerData.runMaxSpeed)
            {
                testTime = soPlayerData.runDeccelTime;
            }

            if (testSpeed < soPlayerData.walkMaxSpeed)
            {
                testTime = testDeccelTime * (1 - testNormalizedTime);
            }
            
            if (testSpeed >= soPlayerData.walkMaxSpeed)
            {
                testTime = soPlayerData.runDeccelTime * (1 - testNormalizedTime);
            }

            //testTime = runDeccelTime; //* (1 - testNormalizedTime);

            IsRunning = false;
            WasRunning = true;
        }   
    }
    private void Jump_started (InputAction.CallbackContext context)
    {
        //Debug.Log(context);
        //Ensures we can't call Jump multiple times from one press
        LastOnGroundTime = 0;
        //_canJump = false;
        IsJumping = true;
        LastPressedJumpTime = soPlayerData.jumpInputBufferTime;
    }

    private void Jump_canceled (InputAction.CallbackContext context)
    {
        if (CanJumpCut())
        {
            _isJumpCut = true;
        }
    }

    private void Teleport_started(InputAction.CallbackContext context)
    {
        //Debug.Log(context);
        LeftClick = true;
        teleportCharge = StartCoroutine(DelayedAction(context));        
    }

    private IEnumerator DelayedAction(InputAction.CallbackContext context)
    {
        yield return new WaitForSeconds(TeleportChargeTime);
        Teleport();
    }

    private void Teleport_canceled(InputAction.CallbackContext context)
    {
        //Debug.Log(context);
        LeftClick = false;
        HasTeleported = false;

        StopCoroutine(teleportCharge);
        //Debug.Log("COROUTINE STOPPED");
    }

    private void Shoot_started(InputAction.CallbackContext context)
    {
        bat.Shoot();
    }

    private void Shoot_canceled(InputAction.CallbackContext context)
    {
        
    }

#endregion

    private void Run(float LerpAmount)
    {
        //float targetSpeed = playerControls.InGame.Walk.ReadValue<float>() * runMaxSpeed;
        //targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, LerpAmount); 
    
        //float accelRate;

        if (LastOnGroundTime > 0)
        {
            //accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;    
        }
        else
        {
            //accelRate = 0;
        }

        //if(doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
		{
			//Prevent any deceleration from happening, or in other words conserve are current momentum
			//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
			//accelRate = 10; 
		}

        //float speedDif = targetSpeed - rb.velocity.x;

        //float movement = speedDif * accelRate;

        //rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Turn()
	{
		//stores scale and flips the player along the x axis, 
		Vector3 scale = transform.localScale; 
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingRight = !IsFacingRight;
        LastPressedWalkTime = 0;
	}

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            transform.SetParent(null);
        }
        
        if (other.gameObject.CompareTag("Platform"))
        {
           ContactPoint2D contact = other.contacts[0];
           
           if (contact.normal.y > 0.5f)
            {
                transform.SetParent(other.gameObject.transform);
            }
        }     

        if (other.gameObject.CompareTag("Death"))
        {
            Die();
            
        }   
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            transform.SetParent(null);
        }        
    }

    public void Die()
    {
        MMF_Particles death = feedbacks.GetFeedbackOfType<MMF_Particles>(searchedLabel: "VFX_Death");
        
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.enabled = false;
        }
        death.Play(transform.position, 1);
        OnDisable();
        HasDied = true;
    }

    public void Jump(InputAction.CallbackContext context = default)
    {
        //Debug.Log("Jump " + context);
        MMF_AudioSource jumpSfx = feedbacks.GetFeedbackOfType<MMF_AudioSource>(searchedLabel:"Jump");
        jumpSfx.Play(transform.position, 1);
        
        walkSmoke.Stop(transform.position, 1);
        jumpSmoke.Play(transform.position, 1);

        if (context.performed)
        {
            if (_canJump == true)
            {
            rb.AddForce(Vector2.up * soPlayerData.jumpSpeed, ForceMode2D.Impulse);
            _canJump = false;
            //Debug.Log("Jump " + context.phase);    
            }
        }
    }

    public void Teleport()
    {
        //Debug.Log("Teleport " + context);
        
        follower.SetPercent(1);
        HasTeleported = true;
    }

    public void PlaySfxFootsteps()
    {
        MMF_AudioSource footstep = feedbacks.GetFeedbackOfType<MMF_AudioSource>(searchedLabel:"Footsteps");

        footstep.Play(transform.position, 1);
    }

    #region CHECK METHODS
    public void checkDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
        {
            Turn();
            IsRunning = false;            
        }
    }

    //public bool CanJump()
    //{
    //    Debug.Log("CanJump() called");
    //    return LastOnGroundTime > 0 && !IsJumping;
    //}

    private bool CanJumpCut()
    {
        return IsJumping && rb.velocity.y > 0;
    }
    #endregion
}