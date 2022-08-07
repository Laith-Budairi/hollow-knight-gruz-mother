using UnityEngine;
using System.Collections;
public class GruzMother : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] Transform player;
    [SerializeField] Transform groundCheckUp;
    [SerializeField] Transform groundCheckDown;
    [SerializeField] Transform wallCheck;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;

    private Vector2 flyDirection;
    private Vector2 attackDirection;
    private Vector2 lastVelocity;
    private Vector3 GroundCheckUpOriginalPosition;
    private Vector3 GroundCheckDownOriginalPosition;

    private float wallCheckRadius = 0.02f;
    private float groundCheckRadius = 0.02f;
    private float groundCheckTimer = 0.5f;
    private const float chargeSpeed = 190;
    private const float attackForce = 250;

    private int attackTimes;

    private byte flySpeed = 50;
    private const byte collisionTimer = 1;

    private bool isFacingRight;
    private bool isTouchingGround;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GroundCheckUpOriginalPosition = groundCheckUp.localPosition;
        GroundCheckDownOriginalPosition = groundCheckDown.localPosition;
        groundCheckTimer = 0.5f;
    }

    private void Update()
    {
        lastVelocity = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (animator.GetBool("isChargingAttack"))
        {
            animator.SetBool("isChargingAttack", false);


            float speed = lastVelocity.magnitude;
            Vector2 direction = Vector2.Reflect(lastVelocity.normalized, other.contacts[0].normal);
            rb.velocity = direction * Mathf.Max(speed, 0f);
            StartCoroutine(ReboundForce());

        }
        else if (animator.GetBool("isUpAndDownAttack"))
        {
            if (other.gameObject.layer == 3) //Ground / Ceiling
                isTouchingGround = true;
            else if (other.gameObject.layer == 6) //Wall
            {
                attackDirection.x *= -1;
                Flip();
            }
        }
    }

    IEnumerator ReboundForce()
    {
        yield return new WaitForSeconds(0.1f);
        rb.velocity = Vector2.zero;
        rb.GetComponent<Animator>().Play("GruzMother_Pause");
    }

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(1f);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(groundCheckUp.position, groundCheckRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheckDown.position, groundCheckRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(wallCheck.position, wallCheckRadius);
    }

    public void FlyStartState()
    {
        //Determine Random Direction
        int leftOrRight = Random.Range(0, 2);
        if (leftOrRight == 0)
        {
            isFacingRight = false;
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else
        {
            isFacingRight = true;
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    public void Fly()
    {
        if (Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer))
        {
            Flip();
            flyDirection.Set(-flyDirection.x, Random.Range(-5, 5));

        }
        if (Physics2D.OverlapCircle(groundCheckUp.position, groundCheckRadius, groundLayer))
        {
            groundCheckUp.localPosition = Vector3.zero;
            flyDirection.Set(!isFacingRight ? Random.Range(-5, 0) : (Random.Range(1, 5)), -flyDirection.y);
        }
        if (Physics2D.OverlapCircle(groundCheckDown.position, groundCheckRadius, groundLayer))
        {
            groundCheckDown.localPosition = Vector3.zero;
            flyDirection.Set(!isFacingRight ? Random.Range(-5, 0) : (Random.Range(1, 5)), -flyDirection.y);

        }
        if (flyDirection.Equals(Vector2.zero))
        {
            if (!isFacingRight)
                flyDirection.Set(Random.Range(-5, 0), Random.Range(-5, 5));
            else
                flyDirection.Set(Random.Range(0, 5), Random.Range(-5, 5));
        }

        rb.velocity = (flyDirection * flySpeed / flyDirection.magnitude) * Time.fixedDeltaTime;
        groundCheckTimer -= Time.deltaTime;

        if (groundCheckTimer <= 0)
        {
            groundCheckUp.localPosition = GroundCheckUpOriginalPosition;
            groundCheckDown.localPosition = GroundCheckDownOriginalPosition;
            groundCheckTimer = 0.5f;
        }
    }

    public void FlyExitState()
    {
        groundCheckUp.localPosition = GroundCheckUpOriginalPosition;
        groundCheckDown.localPosition = GroundCheckDownOriginalPosition;
        rb.velocity = Vector2.zero;
        flyDirection = Vector2.zero;

        float bossXDirection = transform.position.x - player.position.x;
        // Determine Gruz Mother facing direction
        if (bossXDirection > 0 && isFacingRight)
        {
            Flip();
        }
        else if (bossXDirection < 0 && !isFacingRight)
        {
            Flip();
        }
    }

    public void PauseStartState()
    {
        isTouchingGround = false;
        groundCheckUp.localPosition = GroundCheckUpOriginalPosition;
        groundCheckDown.localPosition = GroundCheckDownOriginalPosition;
        rb.velocity = Vector2.zero;
        animator.SetBool("isChargingAttack", false);
        animator.SetBool("isUpAndDownAttack", false);
    }

    public void ChargeAttack()
    {
        animator.SetBool("isChargingAttack", true);
        //Get components and children of Gruz Mother game object

        Vector3 direction = player.position - transform.position;

        rb.velocity = direction.normalized * chargeSpeed * Time.fixedDeltaTime;
    }

    public void AttackUpDownStartState()
    {
        groundCheckUp.localPosition = Vector3.zero;
        groundCheckDown.localPosition = Vector3.zero;

        if (attackTimes == 0)
        {
            attackTimes = Random.Range(15, 20);
            attackDirection = new Vector2(1, 4);
            attackDirection.Normalize();

            if (!isFacingRight)
                Flip();

        }
        else if (attackTimes == 1)
        {
            attackTimes = 0;
            animator.Play("GruzMother_Pause");
        }
    }

    public void AttackUpDown()
    {
        if (isTouchingGround)
        {
            attackDirection.y *= -1;
            isTouchingGround = !isTouchingGround;
            attackTimes--;
            Debug.Log("Attack" + attackTimes);
            animator.Play("GruzMother_Smashed");
        }

        rb.velocity = attackDirection * attackForce * Time.fixedDeltaTime;

    }

    public void AttackUpDownExitState()
    {

        if (attackTimes == 1)
        {
            Debug.Log("SLOWDOWN");
            StartCoroutine(WaitTime());
        }
        rb.velocity = Vector2.zero;
    }

    private void Flip()
    {
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        isFacingRight = !isFacingRight;
    }

    public void determineNextAttack()
    {
        int nextState = 0;

        if (nextState == 1)
            animator.SetBool("isChargingAttack", true);

        else
            animator.SetBool("isUpAndDownAttack", true);
    }
}