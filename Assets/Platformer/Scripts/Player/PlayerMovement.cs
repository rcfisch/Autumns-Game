using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{   
    public InputAction playerControls;
    public InputAction playerJump;
    public InputAction playerHit;
    private Vector2 Input;
    [Header("Player Input and Logic")]
    [SerializeField]private float jumpForce;
    public float targetSpeed;
    private bool isWallSliding;
    [SerializeField]private float wallJumpForceX;
    [SerializeField]private float wallJumpForceY;
    private bool facingRight;
    private bool facingAtJump;
    [SerializeField]private float wallJumps;
    private float wallJumpsCount;
    private bool notDead = true;
    //to prevent you from constantly jumping while holding down A
    private bool alreadyJumped;

    [Header("Player Speed")]
    [SerializeField]private float airMultiplier;
    [SerializeField]private float maxFallingSpeed;
    [SerializeField]private float maxUpwardsSpeed;
    [SerializeField]private float wallSlidingSpeed;
    [SerializeField]private float playerSpeed;
    private float defaultPlayerGravity;
    [SerializeField]private float fallingGravityScale;

    [Header("Components")]
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private Animator anim;
    private SpriteRenderer sr;

    [Header("Delays and timers")]
    public float respawnDelay;
    private float jumpDelay;
    private float cotoyeTimeCounter;
    [SerializeField]private float cotoyeTime;
    private float wallTimeCounter;
    [SerializeField]private float wallTime;
    private bool canJumpCancel;
    private bool startFlashing = true;
    [SerializeField]private float flashStartCount;

    [Header("Jump Buffering")]
    private float jumpBufferCounter;
    [SerializeField]private float jumpBuffer;
    private float wallJumpBufferCounter;
    [SerializeField]private float wallJumpBuffer;

    [Header("GameObjects and Vectors")]
    public GameObject respawnPoint;
    [SerializeField]private Transform wallCheck;
    [SerializeField]private Transform groundCheck;

    [Header("Boxcasting and Sphere Overlap")]
    [SerializeField]private float playerWallCheck;
    [SerializeField]private float playerGroundCheck;
    [SerializeField]private LayerMask GroundLayer;
    [SerializeField]private LayerMask WallLayer;
    private void Awake()
    {
        wallJumpsCount = wallJumps;
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        defaultPlayerGravity = rb.gravityScale;
    }
    private void FixedUpdate()
    {
        StaminaWarning();
    }
    private void Update()
    {
        Jump();
        Movement();
        Animations();
        WallSliding();
        WallJumping();
        JumpGravityScale();
    }
    
    private void Movement()
    {
        Input = new Vector2(Mathf.Round(playerControls.ReadValue<Vector2>().x), Mathf.Round(playerControls.ReadValue<Vector2>().y));
        if(IsGrounded()){
            float speedDiff = targetSpeed - rb.velocity.x;
            cotoyeTimeCounter = cotoyeTime;
        }else{
            float speedDiff = targetSpeed/2 - rb.velocity.x;
            cotoyeTimeCounter -= Time.deltaTime;
        }
        Debug.Log(Input);
        if(IsGrounded()){
            rb.AddForce(transform.right * Input.x * targetSpeed * playerSpeed * Time.deltaTime, ForceMode2D.Force);
        }else{
            rb.AddForce(transform.right * Input.x * targetSpeed * playerSpeed * airMultiplier * Time.deltaTime, ForceMode2D.Force);
        }
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxUpwardsSpeed, maxFallingSpeed ));
    }

    public void Jump()
    {

        if(playerJump.ReadValue<float>() > 0 && alreadyJumped == false)
        {
            jumpBufferCounter = jumpBuffer;
        } else{
            jumpBufferCounter -= Time.deltaTime;
        }
        if(playerJump.ReadValue<float>() > 0 && !IsGrounded()){
            alreadyJumped = true;
        }
        if(IsGrounded()){
            cotoyeTimeCounter = cotoyeTime;
        }else{
            cotoyeTimeCounter -= Time.deltaTime;
        }
        jumpDelay -= Time.deltaTime;
        if(jumpBufferCounter >= 0 && IsGrounded() || jumpBufferCounter >= 0 && cotoyeTimeCounter > 0)
        {   
            //rb.gravityScale = playerGravity * 1f;
            if(jumpDelay <= 0f && alreadyJumped == false)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpDelay = 0.3f;
                canJumpCancel = true;
                cotoyeTimeCounter = 0;
                jumpBufferCounter = 0f;
                wallJumpBufferCounter = 0.1f;
                alreadyJumped = true;
            }
        }
                //reset jump buffer when the A button is no longer pressed
         if(playerJump.ReadValue<float>() == 0)
         {
            alreadyJumped = false;
         }
        if(playerJump.ReadValue<float>() == 0 && canJumpCancel)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y/1.5f);
            canJumpCancel = false;
        } 
    }
    private void WallJumping()
    {
        if(playerJump.ReadValue<float>() > 0)
        {
            wallJumpBufferCounter = jumpBuffer;
        } else{
            wallJumpBufferCounter -= Time.deltaTime;
        }
        if(IsGrounded())
        {
            wallJumpsCount = wallJumps;
        }
        if(IsOnWall()){
            wallTimeCounter = wallTime;
            facingAtJump = facingRight;
        }else{
            wallTimeCounter -= Time.deltaTime;
        }
        if(wallJumpBufferCounter > 0 && isWallSliding && wallJumpsCount > 0 || wallJumpBufferCounter > 0 && wallTimeCounter > 0 && wallJumpsCount > 0)
        {
            if(jumpDelay <= 0)
            {
                rb.AddForce(transform.up * wallJumpForceY, ForceMode2D.Impulse);
                if(facingAtJump == true)
                {
                    rb.AddForce(transform.right * -wallJumpForceX, ForceMode2D.Impulse);
                } else{
                    rb.AddForce(transform.right * wallJumpForceX, ForceMode2D.Impulse);
                }
                wallJumpsCount -= 1f;
                jumpDelay = 0.3f;
                canJumpCancel = true;
                cotoyeTimeCounter = 0;
                wallTimeCounter = 0;
                wallJumpBufferCounter = 0f;
                jumpBufferCounter = 0f;
            }
        }
    }
    private void WallSliding()
    {
       if(IsOnWall() && !IsGrounded())
       {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            isWallSliding = true;
       } else{
            isWallSliding = false;
       }
    }

    private void JumpGravityScale()
    {
        if(rb.velocity.y <= 0 && !IsGrounded() && !IsOnWall())
        {
            rb.gravityScale = defaultPlayerGravity * fallingGravityScale;
        }else{
            rb.gravityScale = defaultPlayerGravity;
        }
    }
    public void Death()
    {
        rb.velocity = new Vector3(0f, 0f, 0f);
        rb.isKinematic = true;
        bc.enabled = false;
        notDead = false;
        anim.SetBool("Death", true);
        anim.SetBool("Falling", false);
        anim.SetBool("WallSliding", false);
        StartCoroutine(RespawnDelay());
    }

    IEnumerator RespawnDelay()
    {
        yield return new WaitForSeconds(1.4f);
        transform.position = respawnPoint.transform.position;
        anim.SetBool("Death", false);
        yield return new WaitForSeconds(1.6f);
        rb.isKinematic = false;
        bc.enabled = true;
        notDead = true;
        jumpBufferCounter = 0f;

    }
    private void Animations()
    {
        if(notDead == true)
        {
            if(IsGrounded())
            {
                anim.SetBool("Falling", false);
                if(Mathf.Abs(rb.velocity.x) > 0.1f)
                {
                    anim.SetBool("Running", true);
                }else{
                    anim.SetBool("Running", false);
                }
            } else{
                anim.SetBool("Running", false);
                anim.SetBool("Falling", true);
            }
            if(IsOnWall() && !IsGrounded())
            {
                anim.SetBool("WallSliding", true);
                anim.SetBool("Falling", false);
            } else{
                anim.SetBool("WallSliding", false);
            }
            if(playerControls.ReadValue<Vector2>().x > 0){
                transform.localScale = new Vector3(1, transform.localScale.y, 1);
                facingRight = true;

            } else if(playerControls.ReadValue<Vector2>().x < 0){
                transform.localScale = new Vector3(-1, transform.localScale.y, 1);
                facingRight = false;
            }
        }
    }
    private void StaminaWarning()
    {
        if(wallJumpsCount <= flashStartCount && startFlashing == true)
        {
            StartCoroutine(Flashing());
            startFlashing = false;
        } 
    }
    IEnumerator Flashing()
    {
        sr.color = new Color(1, 0.7f, 0.7f);
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        startFlashing = true;
    }
    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, playerGroundCheck, GroundLayer);
        //Debug.Log(raycastHit.collider.gameObject.name);
        return raycastHit;
    }
    private bool IsOnWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, playerWallCheck, WallLayer);
    }
    private void OnEnable()
    {
        playerControls.Enable();
        playerHit.Enable();
        playerJump.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
        playerHit.Disable();
        playerJump.Disable();
    }
} 