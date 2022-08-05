using UnityEngine;

public class GruzMotherFly : StateMachineBehaviour
{
    private Transform gruzMother;
    private Rigidbody2D rb;

    private Transform groundCheckUp;
    private Transform groundCheckDown;
    private Transform wallCheck;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;

    private Vector2 flyDirection;
    private Vector3 GroundCheckUpOriginalPosition;
    private Vector3 GroundCheckDownOriginalPosition;

    private float wallCheckRadius = 0.02f;
    private float groundCheckRadius = 0.02f;
    private float stateTimer = 3;
    private float groundCheckTimer;

    private byte flySpeed = 50;

    private bool isFacingRight;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Get components and children of Gruz Mother game object
        gruzMother = GameObject.FindGameObjectWithTag("Boss").GetComponent<Transform>();
        rb = gruzMother.GetComponent<Rigidbody2D>();
        groundCheckUp = gruzMother.Find("GroundCheckUp");
        groundCheckDown = gruzMother.Find("GroundCheckDown");
        wallCheck = gruzMother.Find("WallCheck");

        GroundCheckUpOriginalPosition = groundCheckUp.localPosition;
        GroundCheckDownOriginalPosition = groundCheckDown.localPosition;
        groundCheckTimer = 0.5f;


        //Determine Gruz Mother direction
        if (gruzMother.localScale.x < 0)
            isFacingRight = true;
        else
            isFacingRight = false;


        //Determine Gruz Mother facing direction
        if (!isFacingRight && GameObject.FindObjectOfType<Fox>().transform.position.x >= gruzMother.position.x)
        {
            gruzMother.localScale = new Vector2(-gruzMother.localScale.x, gruzMother.localScale.y);
            isFacingRight = true;

        }


        //Determine Gruz Mother facing direction
        if (isFacingRight && GameObject.FindObjectOfType<Fox>().transform.position.x < gruzMother.position.x)
        {
            gruzMother.localScale = new Vector2(-gruzMother.localScale.x, gruzMother.localScale.y);
            isFacingRight = false;
        }

        //Determine Random Direction
        ChangeDirectionRandomly();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If timer finishes then move to the next state
        if (stateTimer > 0)
        {
            stateTimer -= Time.deltaTime;

            //Gruz Mother fly function
            Fly();

        }
        else
        {
            animator.Play("GruzMother_Anticipation");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        groundCheckUp.localPosition = GroundCheckUpOriginalPosition;
        groundCheckDown.localPosition = GroundCheckDownOriginalPosition;
        rb.velocity = Vector2.zero;
        flyDirection = Vector2.zero;
        stateTimer = 3;

        //Determine Gruz Mother facing direction
        if (!isFacingRight && GameObject.FindObjectOfType<Fox>().transform.position.x > gruzMother.position.x)
        {
            gruzMother.localScale = new Vector2(-gruzMother.localScale.x, gruzMother.localScale.y);
            isFacingRight = !isFacingRight;

        }


        //Determine Gruz Mother facing direction
        if (isFacingRight && GameObject.FindObjectOfType<Fox>().transform.position.x < gruzMother.position.x)
        {
            gruzMother.localScale = new Vector2(-gruzMother.localScale.x, gruzMother.localScale.y);
            isFacingRight = !isFacingRight;
        }
    }

    private void ChangeDirectionRandomly()
    {
        int leftOrRight = Random.Range(0, 2);

        if (leftOrRight == 0)
        {
            isFacingRight = !isFacingRight;
            gruzMother.localScale = new Vector2(-gruzMother.localScale.x, gruzMother.localScale.y);
        }


    }

    private void Fly()
    {
        if (Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer))
        {
            gruzMother.localScale = new Vector2(-gruzMother.localScale.x, gruzMother.localScale.y);
            isFacingRight = !isFacingRight;
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
}
