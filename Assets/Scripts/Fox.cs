using System.Collections;
using System;
using UnityEngine;

public class Fox : MonoBehaviour
{

    public static event Action hasLanded;
    //Private Fields - Components
    private Rigidbody2D rb;
    private Animator animator;
    [SerializeField] Collider2D standingCollider, crouchingCollider;
    [SerializeField] Transform overheadCheckCollider;
    [Header("GroundCheck")]
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform wallCheckCollider;
    [SerializeField] LayerMask wallLayer;

    [SerializeField] private float crouchSpeedModifier = 0.5f;
    [SerializeField] float speed = 1;
    [SerializeField] float jumpPower = 50;
    [SerializeField] float slideFactor = 0.02f;
    const float groundCheckRadius = 0.02f;
    const float overheadCheckRadius = 0.02f;
    const float wallCheckRadius = 0.02f;
    private float horizontalValue;
    private float runSpeedModifier = 2f;

    [SerializeField] int totalJumps;
    int availableJumps;

    [SerializeField] private bool isRunning;
    [SerializeField] private bool isGrounded;
    [SerializeField] bool crouch;
    private bool facingRight = true;
    bool releasedJumpButton = true;
    bool multipleJump;
    bool coyoteJump;
    bool isSliding;
    bool isDead;

    void Awake()
    {
        availableJumps = totalJumps;
        Application.targetFrameRate = -1;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

    }

    void Update()
    {
        if (!CanMoveOrInteract())
            return;
        //Set the yVelocity in the animator
        animator.SetFloat("yVelocity", rb.velocity.y);

        // Store the horizontal value
        horizontalValue = Input.GetAxisRaw("Horizontal");
        // If LShift is clicked (enable is running)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        // If LShift is released (disable is running)
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }

        //If we press jump button then enable it
        if (Input.GetButtonDown("Jump"))
            Jump();
        //Otherwise disable it
        else if (Input.GetButtonUp("Jump"))
        {
            releasedJumpButton = true;
        }

        //If we press crouch button then enable it
        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        //Otherwise disable it
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

        //Check if we are touching a wall to slide on it
        WallCheck();

    }

    void FixedUpdate()
    {
        GroundCheck();
        Move(horizontalValue, crouch);
    }

    void Jump()
    {
        if (isGrounded)
        {
            multipleJump = true;
            availableJumps--;

            //If the player is grounded and pressed space -> jump
            if (releasedJumpButton)
            {
                releasedJumpButton = false;
                rb.velocity = Vector2.up * jumpPower;
                animator.SetBool("Jump", true);
            }

        }
        else
        {
            if (coyoteJump)
            {
                multipleJump = true;
                if (releasedJumpButton)
                {
                    releasedJumpButton = false;
                    rb.velocity = Vector2.up * jumpPower;
                    animator.SetBool("Jump", true);
                }
            }

            if (multipleJump && availableJumps > 0)
            {
                availableJumps--;
                rb.velocity = Vector2.up * jumpPower;
                animator.SetBool("Jump", true);
            }
        }

    }

    void Move(float direction, bool crouchFlag)
    {
        #region Crouch

        // If we are crouching and disabled crouching
        // Check overhead for collision with Ground items
        // If there are any, remain crouched, otherwise un-crouch
        if (!crouchFlag)
        {
            if (Physics2D.OverlapCircle(overheadCheckCollider.position, overheadCheckRadius, groundLayer))
            {
                crouchFlag = true;
            }
        }
        animator.SetBool("Crouch", crouchFlag);
        standingCollider.enabled = !crouchFlag;
        crouchingCollider.enabled = crouchFlag;
        #endregion

        #region Move & Run
        // Set value of x using direction and speed
        float xVal = direction * speed * Time.fixedDeltaTime;

        // If we are running, multiply with the running modifier
        if (isRunning)
        {
            xVal *= runSpeedModifier;
        }
        // If we are crouching, multiply with the crouching modifier

        if (crouchFlag)
        {
            xVal *= crouchSpeedModifier;
        }

        // Create Vec2 for the velocity
        Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
        // Set the player's velocity
        rb.velocity = targetVelocity;



        // If looking right and clicked left (flip to the left)
        if (facingRight && direction < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            facingRight = false;
        }
        // If looking left and clicked right (flip to the right)
        else if (!facingRight && direction > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            facingRight = true;
        }

        // 0 idle, 0.4 walking, 0.8 running
        //Set the float xVelocity according to the x value of the rigid body velocity
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        #endregion

    }

    void GroundCheck()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false;
        // Check if the GroundCheckObject is colliding with other
        //2D Colliders that are in the "Ground" Layer

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0)
        {
            isGrounded = true;
            if (!wasGrounded)
            {
                availableJumps = totalJumps;
                multipleJump = false;
                //Trigger landed event
                hasLanded?.Invoke();
            }

            //Check if any of the colliders is moving platform
            foreach(var c in colliders)
            {
                if(c.tag.CompareTo("MovingPlatform") == 0)
                {
                    transform.parent = c.transform;
                }
            }
            //Parent it to this transform
        }
        else
        {
            //Un=parent the transform
            transform.parent = null;
            if (wasGrounded)
            {
                StartCoroutine(CoyoteJumpDelay());
            }
        }

        //As long as we are grounded the "Jump" bool
        //in the animator is disabled
        animator.SetBool("Jump", !isGrounded);
    }

    void WallCheck()
    {
        //If we are touching a wall
        //and we are moving towards the wall
        //and we are not grounded
        //Slide on the wall
        if (Physics2D.OverlapCircle(wallCheckCollider.position, wallCheckRadius, wallLayer) && Mathf.Abs(horizontalValue) > 0 && rb.velocity.y < 0 && !isGrounded)
        {
            if(!isSliding)
            {
                availableJumps = totalJumps;
                multipleJump = true;
            }
            Vector2 v = rb.velocity;
            v.y = -slideFactor;
            v.x = 0;
            rb.velocity = v;
            isSliding = true;

            if (Input.GetButtonDown("Jump"))
            {
                availableJumps--;

                rb.velocity = Vector2.up * jumpPower;
                animator.SetBool("Jump", true);

            }

        }
        else
        {
            isSliding = false;
        }
    }

    IEnumerator CoyoteJumpDelay()
    {
        coyoteJump = true;
        yield return new WaitForSeconds(0.2f);
        coyoteJump = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheckCollider.position, groundCheckRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(overheadCheckCollider.position, overheadCheckRadius);
    }

    public bool CanMoveOrInteract()
    {
        bool can = true;

        if (isDead)
            can = false;

        return can;
    }

    public void Die()
    {
        isDead = true;
    }

    public void ResetPlayer()
    {
        horizontalValue = 0;
        isDead = false;
    }


}
