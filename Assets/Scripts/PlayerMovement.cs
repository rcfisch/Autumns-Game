using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   
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

    [Header("Player Speed")]
    [SerializeField]private float airMultiplier;
    [SerializeField]private float maxFallingSpeed;
    [SerializeField]private float maxUpwardsSpeed;
    [SerializeField]private float wallSlidingSpeed;
    [SerializeField]private float playerSpeed;

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
    }
    
    private void Movement()
    {
        float HorizontalInput = Input.GetAxisRaw("Horizontal");
        if(IsGrounded()){
            float speedDiff = targetSpeed - rb.velocity.x;
            cotoyeTimeCounter = cotoyeTime;
        }else{
            float speedDiff = targetSpeed/2 - rb.velocity.x;
            cotoyeTimeCounter -= Time.deltaTime;
        }

        if(IsGrounded()){
            rb.AddForce(transform.right * HorizontalInput * targetSpeed * playerSpeed * Time.deltaTime, ForceMode2D.Force);
        }else{
            rb.AddForce(transform.right * HorizontalInput * targetSpeed * playerSpeed * airMultiplier * Time.deltaTime, ForceMode2D.Force);
        }
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxUpwardsSpeed, maxFallingSpeed ));
    }

    public void Jump()
    {
        if(Input.GetKeyDown("space"))
        {
            jumpBufferCounter = jumpBuffer;
        } else{
            jumpBufferCounter -= Time.deltaTime;
        }
        if(IsGrounded()){
            cotoyeTimeCounter = cotoyeTime;
        }else{
            cotoyeTimeCounter -= Time.deltaTime;
        }
        jumpDelay += Time.deltaTime;
        if(jumpBufferCounter >= 0 && IsGrounded() || jumpBufferCounter >= 0 && cotoyeTimeCounter > 0)
        {   
            //rb.gravityScale = playerGravity * 1f;
            if(jumpDelay >= 0.1)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpDelay = 0f;
                canJumpCancel = true;
                cotoyeTimeCounter = 0;
            }
        }
        if(Input.GetKeyUp("space") && canJumpCancel)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y/2);
            canJumpCancel = false;
        } 
    }
    private void WallJumping()
    {
        if(Input.GetKeyDown("space"))
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
        if(wallJumpBufferCounter > 0 && isWallSliding && jumpDelay >= 0.1 && wallJumpsCount > 0 || wallJumpBufferCounter > 0 && wallTimeCounter > 0 && jumpDelay >= 0.1 && wallJumpsCount > 0)
        {
            rb.AddForce(transform.up * wallJumpForceY, ForceMode2D.Impulse);
            if(facingAtJump == true)
            {
                rb.AddForce(transform.right * -wallJumpForceX, ForceMode2D.Impulse);
            } else{
                rb.AddForce(transform.right * wallJumpForceX, ForceMode2D.Impulse);
            }
            wallJumpsCount -= 1f;
            jumpDelay = 0f;
            canJumpCancel = true;
            cotoyeTimeCounter = 0;
            wallTimeCounter = 0;
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
            if(Input.GetAxisRaw("Horizontal") > 0){
                transform.localScale = new Vector3(1, transform.localScale.y, 1);
                facingRight = true;

            } else if(Input.GetAxisRaw("Horizontal") < 0){
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
        //RaycastHit2D raycastHit = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, playerJumpCheck, GroundLayer);
        //Debug.Log(raycastHit.collider.gameObject.name);
        return Physics2D.OverlapCircle(groundCheck.position, playerGroundCheck, GroundLayer);
    }
    private bool IsOnWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, playerWallCheck, WallLayer);
    }
} 