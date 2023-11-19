//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//[RequireComponent(typeof(AudioSource))]
//[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(TrailRenderer))]
//[RequireComponent(typeof(BoxCollider2D))]
//[RequireComponent(typeof(PlatformerController2D))]
//public class PlayerController : MonoBehaviour
//{

//    public static PlayerController Player { get; private set; }

//    private SpriteRenderer spriteRenderer;

//    public AudioSource source;
    
//    public AudioClip jumpSound;
//    public AudioClip dashSound;
//    public AudioClip idleSound;
//    public AudioClip runSound;
//    public AudioClip slideSound;
//    public AudioClip wallclimbSound;
//    public AudioClip wallslideSound;
//    public AudioClip fallingSound;
//    public AudioClip landSound;
//    private string currentAudioState;

//    public bool visualiserOn = true;

//    private TrailRenderer trail;
//    private Rigidbody2D rb;
//    private PlatformerController2D col;
//    private BoxCollider2D bc;
//    public LayerMask collisionLayer;

//    public LayerMask JumpingLayerMask;

//    #region Variables

    
//    [Header("Jumping")]
//    private bool jumpInput;
//    private int maxJumps = 2;
//    private int jumpsLeft;
//    public float jumpSpeed = 100f;
//    private bool jumpReleaseInput;

//    [Header("Airtime settings")]
//    public float normalGravityModifier = 1.2f;
//    public float releaseGravityModifier = 3f;
//    public float baseGravityScale = 2.5f;
//    public float hangtimeGravity = 0.1f;


    
//    [Header("Movement")]
//    public float inputTreshold = 0.4f;
//    public float runMaxSpeed = 8f; //Target speed we want the player to reach.
//    public float runAcceleration = 8f; //Time (approx.) time we want it to take for the player to accelerate from 0 to the runMaxSpeed.
//    [HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
//    public float runDecceleration = 0.5f; //Time (approx.) we want it to take for the player to accelerate from runMaxSpeed to 0.
//    [HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
//    [Space(10)]
//    [Range(0.01f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
//    [Range(0.01f, 1)] public float deccelInAir;
//    public bool doConserveMomentum;
//    private float horizontalMovementInputRaw;
//    private float horizontalMovementInput;

//    public float dir = -1; // Ensimmainen suunta on vasemmalle


//    [Header("Dash")]
//    private bool dashInput;
//    public float dashingTime = 0.25f;
//    public float dashingCooldown = 0.7f;
//    public float dashPower = 8000f;
//    private bool isDashing = false;
//    private bool canDash = true;


//    [Header("Wall Climb")]
//    public float WallclimbSpeed = 3f;
//    private bool VerticalMovementInput;
//    public float wallJumpAngle = 25f;
//    public float baseWallJumpSpeed = 60f;
//    public float extraWallJumpSpeed = 20f;
//    Vector2 wallJumpSpeedVector;
//    private bool isWallClimbing;
//    private bool isClimbingCorner = false;



//    [Header("Animations")]
//    Animator animator;
//    private string currentState;
//    public AnimationClip PLAYER_IDLE_ASSIGN;
//    private string PLAYER_IDLE;

//    public AnimationClip PLAYER_WALK_ASSIGN;
//    private string PLAYER_WALK;

//    public AnimationClip PLAYER_JUMP_ASSIGN;
//    private string PLAYER_JUMP;

//    public AnimationClip PLAYER_AIRTIME_ASSIGN;
//    private string PLAYER_AIRTIME;

//    public AnimationClip PLAYER_FALL_ASSIGN;
//    private string PLAYER_FALL;

//    public AnimationClip PLAYER_LAND_ASSIGN;
//    private string PLAYER_LAND;

//    public AnimationClip PLAYER_SLIDE_ASSIGN;
//    private string PLAYER_SLIDE;

//    public AnimationClip PLAYER_CLIMB_ASSIGN;
//    private string PLAYER_CLIMB;

//    public AnimationClip PLAYER_DASH_ASSIGN;
//    private string PLAYER_DASH;

//    public AnimationClip PLAYER_SLIDE_GROUND_MIDDLE_ASSIGN;
//    private string PLAYER_SLIDE_GROUND_MIDDLE;

//    public AnimationClip PLAYER_SLIDE_GROUND_START_ASSIGN;
//    private string PLAYER_SLIDE_GROUND_START;

//    public AnimationClip PLAYER_SLIDE_GROUND_END_ASSIGN;
//    private string PLAYER_SLIDE_GROUND_END;

//    public AnimationClip PLAYER_CORNER_ASSIGN;
//    private string PLAYER_CORNER;

//    public AnimationClip PLAYER_LANDING_ASSIGN;
//    private string PLAYER_LANDING;

//    [SerializeField]
//    private Vector2 offset1;
//    [SerializeField] 
//    private Vector2 offset2;

//    private Vector2 climbBegunPosition;
//    private Vector2 climbOverPosition;

//    private bool canGrabLedge = true;
//    private bool hasLanded = false;

//    private AnimationEvents animationEvents;
    

//    #endregion

//    private void Awake()
//    {
//        if (Player != null && Player != this)
//        {
//            Destroy(this);
//        }
//        else
//        {
//            Player = this;
//        }
//    }

//    // Kun Peli alkaa niin laske ja aseta muuttujat
//    void Start()
//    {
        
//        SetVariables();
//        Calculations();
//    }

//    // Tanne kaikki inputtaamiseen liittyvat funktiot
//    void Update()
//    {
//        //Debug.Log(" top collission: " + col.collisions.top + " bottom collision: " + col.collisions.bottom);
//        FollowInputs();
//        //Debug.Log("Can grab ledge: " + canGrabLedge);

        
//    }

//    // Tänne kaikki fysiikkaan liittyvat funktiot
//    private void FixedUpdate()
//    {

//        checkForLedge();

//        ClimbOverLedge();

//        if (isClimbingCorner || isDashing) return;

//        Move();

//        Wallclimb();

//        Jump();

//        ResetLeftJumps();

//        WallJump();

//        Dash();

//        ModifyGravity();

//        DeaccelerateAfterSpaceLift();

//        AirLanding();

//        TouchingObstacle();

//        TouchingObstaclePreventMove();
//    }

//    void ChangeAnimationState(string newState)
//    {
//        if (currentState == newState) return;

//        animator.Play(newState);

//        currentState = newState;
//    }

//    void ChangeSoundState(AudioClip audioState, float volume, bool isLooping, bool overide = false)
//    {
//        if (currentAudioState == audioState.name && !overide) return;
//        source.Stop();
//        source.clip = audioState;
//        source.volume = volume;
//        source.loop = isLooping;
//        source.Play();
//        currentAudioState = audioState.name;
        

//    }



//    private void checkForLedge()
//    {
//        //Debug.Log(col.collisions.top + " " + col.collisions.bottom);

//        if (col.collisions.top == false && col.collisions.middle == true && canGrabLedge && rb.velocity.y > 0)
//        {
//            isClimbingCorner = true;
//            canGrabLedge = false;
//            rb.velocity = Vector2.zero;
//            rb.gravityScale = 0;
//            //Debug.Log("Imgere");
            
            

//            Vector2 ledgePosition = transform.position;
//            climbBegunPosition = ledgePosition + offset1;
//            climbOverPosition = new Vector2(ledgePosition.x + (offset2.x*dir), ledgePosition.y + offset2.y);
                
//            transform.position = climbBegunPosition;
//            ChangeAnimationState(PLAYER_CORNER); // Tässä animaatiossa on tapahtuma, joka laukaisee alla olevan funktion
 


            
//        }
//    }

//    private void ClimbOverLedge()
//    {
//        if (!animationEvents.evtClimbOver) return;

//        //Debug.Log(climbOverPosition);
//        transform.position = climbOverPosition;
//        rb.gravityScale = baseGravityScale;
//        isClimbingCorner = false;
//        animationEvents.evtClimbOver = false;
//        ChangeAnimationState(PLAYER_IDLE);
//        Invoke("CanGrabLedge", .4f);
//    }

//    private void CanGrabLedge() => canGrabLedge = true;

//    #region Move

//    private void Move()
//    {
//        if (Mathf.Abs(horizontalMovementInputRaw) < inputTreshold || isWallClimbing == true) 
//        {
//            horizontalMovementInput = 0;
            
//            if (isGrounded() && !isDashing && animationEvents.evtFallingOver)
//            {
//                if (Mathf.Abs(rb.velocity.x) <= 3)
//                {
//                    ChangeAnimationState(PLAYER_IDLE);
//                    ChangeSoundState(idleSound, .1f, true);
//                }
                    
//                else
//                {
//                    ChangeAnimationState(PLAYER_SLIDE_GROUND_START);
//                    ChangeSoundState(slideSound, .1f, false);
//                }
                           
//            }
//            return; 
//        } // jos input ei ole kovempaa kuin treshold niin resettaa input 0, jos dashaat tai kiipeat niin palaa

//        if (isGrounded() && isDashing == false && animationEvents.evtFallingOver)
//        {
//            ChangeAnimationState(PLAYER_WALK);
//            ChangeSoundState(runSound, 1f, true);

//        }
            

//        dir = (horizontalMovementInput < inputTreshold) ? -1 : (horizontalMovementInput > inputTreshold ? 1 : 0);

//        //Calculate the direction we want to move in and our desired velocity
//        float targetSpeed = dir * runMaxSpeed;


//        if (dir == -1)
//            spriteRenderer.flipX = false;
//        else
//            spriteRenderer.flipX = true;
            


//        #region Calculate AccelRate
//        float accelRate = 1;

//        //Gets an acceleration value based on if we are accelerating (includes turning) 
//        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
//        if (isGrounded()==true)
//            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;
//        else if (isGrounded() == false)
//            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount * accelInAir : runDeccelAmount * deccelInAir;
//        #endregion



//        //Calculate difference between current velocity and desired velocity
//        float speedDif = targetSpeed - rb.velocity.x;

//        //Calculate force along x-axis to apply to thr player
//        float movement = speedDif * accelRate;

//        //Convert this to a vector and apply to rigidbody
//        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

//        /*
//		 * For those interested here is what AddForce() will do
//		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
//		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
//		*/

//    } // function



//    private IEnumerator Dash()
//    {
//        canDash = false;
//        isDashing = true;
//        ChangeSoundState(dashSound, 1f, false, true);
//        ChangeAnimationState(PLAYER_DASH);
//        float originalGravity = rb.gravityScale;
//        rb.gravityScale = 0f;
//        rb.AddForce(new Vector2(dir * dashPower, 1000f));
//        rb.velocity = new Vector2(rb.velocity.x, 0);
//        yield return new WaitForSeconds(dashingTime);
//        rb.gravityScale = originalGravity;
//        rb.velocity = new Vector2(rb.velocity.x / 2, 0);
//        isDashing = false;
//        yield return new WaitForSeconds(dashingCooldown);
//        canDash = true;

        

//    } // IEnumerator

//    private void Jump()
//    {
//        if (jumpInput == false) return; // jos et ole painanut jump inputtia et voi hyppaa
//        if (isDashing == true || isWallClimbing == true) return; // jos dashaat et voi hypätä tai kavelet seinaa et voi tehda normaalia hyppya
//        if (isGrounded() == false && jumpsLeft <= 1) return; // Jos oot ilmassa ja ei oo hyppyja jaljella palaa

//        rb.velocity = new Vector2(rb.velocity.x, 0f);

//        ChangeAnimationState(PLAYER_JUMP);
//        ChangeSoundState(jumpSound, 1f, false, true);

//        jumpsLeft--;
//        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);

//        jumpInput = false;

//    }  // function

//    private void AirLanding()
//    {
//        if (!isGrounded()) { hasLanded = false; animationEvents.evtFallingOver = false; return; };
//        if (!canGrabLedge) { hasLanded = true; animationEvents.evtFallingOver = true; return; };


//        if (!hasLanded)
//        {
//            //Debug.Log("imhere");
//            ChangeAnimationState(PLAYER_LANDING);
//            // tämä triggeraa eventin mikä kertoo millon animaation on loppunut
//            ChangeSoundState(landSound, .5f, false);
//            hasLanded = true;
//        }
//    }

//    private void ModifyGravity()
//    {
//        if (isGrounded() == true) { rb.gravityScale = baseGravityScale; return; }  // jos olet maassa gravitaatio voima on normaali
//        if (TouchingWall() == true) { rb.gravityScale = baseGravityScale; return; } // jos olet ilmassa gravitaatio voima on normaali
//        if (!canGrabLedge) return;


//        trail.enabled = true;

//        if (rb.velocity.y > -3 && rb.velocity.y < 3)
//        {
            
//            rb.gravityScale = baseGravityScale * hangtimeGravity;

//            if (isDashing != true)
//            {
//                ChangeAnimationState(PLAYER_AIRTIME);
//                ChangeSoundState(fallingSound, .1f, false);
//            }
                

//            return;
//        }
            

//        if (rb.velocity.y <= -3)
//        {
            
//            rb.gravityScale = baseGravityScale * normalGravityModifier;

//            if (isDashing != true)
//                ChangeAnimationState(PLAYER_FALL);

//            return;
//        }
//    }  // function


//    private void DeaccelerateAfterSpaceLift()
//    {
//        if (TouchingWall() == true || jumpReleaseInput == false) 
//        { rb.gravityScale = baseGravityScale; jumpReleaseInput = false; return; }
//        if (isGrounded() == true || jumpReleaseInput == false)
//        { rb.gravityScale = baseGravityScale; jumpReleaseInput = false; return; }

//        rb.gravityScale = baseGravityScale * releaseGravityModifier;
//    }  // function


//    private void ResetLeftJumps()
//    {
//        if (isGrounded() == false) return;  // jos sä oot maassa niin resettaa hyppy määrät normaaliksi

//        jumpsLeft = maxJumps;
//    }  // function

//    private void TouchingObstaclePreventMove()
//    {
//        if (!TouchingObstacle()) return;

//        Debug.Log("im here bich");

//        if (rb.velocity.x > dir)
//            rb.velocity = new Vector2(0, rb.velocity.y);
//    }





//    // wallclimb
//    private void Wallclimb()
//    {
//        if (isGrounded()) return;
//        if (!TouchingWall()) return;
        
//        if (visualiserOn)
//            visualizeWallJump();

//        trail.enabled = false;

        

//        // implimentaatio
//        isWallClimbing = true;


//        if (VerticalMovementInput)
//        {
//            ChangeAnimationState(PLAYER_CLIMB);
//            ChangeSoundState(wallclimbSound, 1.2f, true);
//            rb.velocity = Vector2.up * WallclimbSpeed;
//        }
//        else
//        {
//            rb.velocity = Vector2.zero;
//            ChangeAnimationState(PLAYER_SLIDE);
//            ChangeSoundState(wallslideSound, .2f, true);
//        }
            
//    } // function



//    // walljump
//    private void WallJump()
//    {
//        if (jumpInput == false) return;
//        if (TouchingWall() == false) return; // jos osut seinään ja jump on true  (false || false)  (true && true)



//        jumpInput = false;
//    }  // function



//    #endregion




//    #region Setup

//    private void Calculations()
//    {
//        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
//        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

//        #region Variable Ranges
//        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
//        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
//        #endregion
//    }

//    private void SetVariables()
//    {
//        animator = GetComponentInChildren<Animator>();
//        trail = GetComponent<TrailRenderer>();
//        rb = GetComponent<Rigidbody2D>();
//        bc = GetComponent<BoxCollider2D>();
//        col = GetComponent<PlatformerController2D>();
//        spriteRenderer = GetComponentInChildren<SpriteRenderer>();





//        animationEvents = GetComponentInChildren<AnimationEvents>();


//        PLAYER_IDLE = PLAYER_IDLE_ASSIGN.name;
//        PLAYER_WALK = PLAYER_WALK_ASSIGN.name;
//        PLAYER_JUMP = PLAYER_JUMP_ASSIGN.name;
//        PLAYER_AIRTIME = PLAYER_AIRTIME_ASSIGN.name;
//        PLAYER_FALL = PLAYER_FALL_ASSIGN.name;
//        PLAYER_DASH = PLAYER_DASH_ASSIGN.name;
//        PLAYER_CLIMB = PLAYER_CLIMB_ASSIGN.name;
//        PLAYER_SLIDE = PLAYER_SLIDE_ASSIGN.name;
//        PLAYER_SLIDE_GROUND_MIDDLE = PLAYER_SLIDE_GROUND_MIDDLE_ASSIGN.name;
//        PLAYER_SLIDE_GROUND_START = PLAYER_SLIDE_GROUND_START_ASSIGN.name;
//        PLAYER_SLIDE_GROUND_END = PLAYER_SLIDE_GROUND_END_ASSIGN.name;
//        PLAYER_CORNER = PLAYER_CORNER_ASSIGN.name;
//        PLAYER_LANDING = PLAYER_LANDING_ASSIGN.name;



//        //trail.SetPosition(1, new Vector3(transform.position.x, transform.position.y - bc.size.y/2, 0));
//        isDashing = false;
//        isWallClimbing = false;
//        jumpInput = false;
//        dashInput = false;
//        jumpsLeft = maxJumps;
//        horizontalMovementInput = 0;

//        JumpingLayerMask = LayerMask.GetMask("JumpingPlatform");

//    }

//    #endregion


//    #region Visualisation

//    private void visualizeWallJump()
//    {


//        float wallJumpSpeedXvector;
//        float wallJumpSpeedYvector;


//        wallJumpSpeedYvector = (extraWallJumpSpeed + baseWallJumpSpeed);
//        wallJumpSpeedXvector = wallJumpSpeedYvector * Mathf.Tan((Mathf.Deg2Rad * wallJumpAngle)) * -dir;


//        wallJumpSpeedVector = new Vector2(wallJumpSpeedXvector, wallJumpSpeedYvector);


//        Debug.DrawRay(
//                new Vector3(bc.bounds.center.x, bc.bounds.center.y, 0),
//                Vector3.Normalize(new Vector3(wallJumpSpeedVector.x, wallJumpSpeedVector.y + rb.velocity.y, 0)) * 4,
//                Color.yellow);
//    }



//    #endregion // vain debuggaukseen


//    #region FollowedValues

//    private void FollowInputs()
//    {

//        if (isClimbingCorner) return;

//        horizontalMovementInputRaw = Input.GetAxis("Horizontal");

//        if (horizontalMovementInputRaw != 0)
//            horizontalMovementInput = horizontalMovementInputRaw;

        

//        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Vertical") > 0)
//            VerticalMovementInput = true;

//        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetAxis("Vertical") <= 0)
//            VerticalMovementInput = false;


        
//        if (!TouchingWall() && canGrabLedge)
//        {
//            if ((Input.GetKeyDown(KeyCode.X) == true || Input.GetKeyDown(KeyCode.Mouse0) == true || Input.GetAxisRaw("Dash") > 0) && canDash == true)
//            {
//                //Debug.Log("I'm touching wall" + TouchingWall());
//                StartCoroutine(Dash());
//            }
//        }
       
            


//        if (Input.GetKeyDown(KeyCode.Space) == true || Input.GetButtonDown("Jump"))
//            jumpInput = true;


//        if (Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Jump"))
//            jumpReleaseInput = true;


//    }

//    public bool isGrounded()
//    {
//        float extraVerticalHeight = bc.edgeRadius/2+.2f;
//        float extraHorizontalHeight = bc.edgeRadius;


//        col.VerticalRaycasts(bc, collisionLayer, extraVerticalHeight, extraHorizontalHeight);

//        // guard clause
//        if (col.collisions.bottomLeft == false && col.collisions.bottomRight == false)
//        {
//            Debug.DrawRay(new Vector3(bc.bounds.center.x - bc.bounds.size.x / 2 - extraHorizontalHeight, bc.bounds.center.y, 0), Vector2.down * (bc.bounds.extents.y + extraVerticalHeight), Color.red);
//            Debug.DrawRay(new Vector3(bc.bounds.center.x + bc.bounds.size.x / 2 + extraHorizontalHeight, bc.bounds.center.y, 0), Vector2.down * (bc.bounds.extents.y + extraVerticalHeight), Color.red);
//            return false;
//        }
//        else
//        {
//            Debug.DrawRay(new Vector3(bc.bounds.center.x - bc.bounds.size.x / 2 - extraHorizontalHeight, bc.bounds.center.y, 0), Vector2.down * (bc.bounds.extents.y + extraVerticalHeight), Color.green);
//            Debug.DrawRay(new Vector3(bc.bounds.center.x + bc.bounds.size.x / 2 + extraHorizontalHeight, bc.bounds.center.y, 0), Vector2.down * (bc.bounds.extents.y + extraVerticalHeight), Color.green);
//            isWallClimbing = false;
//            rb.gravityScale = baseGravityScale;
//            trail.enabled = false;
//            return true;
//        } // if

//    } // function



//    public bool TouchingWall()
//    {
//        if (isGrounded() == true) return false;
//        if (rb.velocity.y >= 1f && isWallClimbing == false) return false;


//        float extraHeight = bc.edgeRadius/2+.2f;

//        col.HorizontalRaycasts(dir, bc, collisionLayer, extraHeight);


//        if (col.collisions.top == false && col.collisions.middle == false)
//        {
//            if (visualiserOn)
//                col.visualizeHorizontalRaysFalse(bc, dir, extraHeight);

//            isWallClimbing = false;
//            return false;
//        }
//        else
//        {
//            if (visualiserOn)
//                col.visualizeHorizontalRaysTrue(bc, dir, extraHeight);
//            jumpsLeft = maxJumps;
//            return true;
//        } // if
//    } // function


//    public bool TouchingObstacle()
//    {
//        if (isGrounded() == true) return false;

//        float extraHeight = bc.edgeRadius / 2 + .2f;

//        col.HorizontalRaycasts(dir, bc, collisionLayer, extraHeight);

//        if (col.collisions.bottom == true && col.collisions.middle == false && col.collisions.top == false)
//        {

//            if (visualiserOn)
//                col.visualizeHorizontalRaysTrue(bc, dir, extraHeight);
//            return true;
           
//        }
//        else
//        {
//            if (visualiserOn)
//                col.visualizeHorizontalRaysFalse(bc, dir, extraHeight);

//            return false;

//        }
//    }
//    #endregion




//} // class