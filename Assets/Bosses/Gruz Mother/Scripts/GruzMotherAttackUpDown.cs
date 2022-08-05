using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruzMotherAttackUpDown : StateMachineBehaviour
{
    private Transform gruzMother;
    private Rigidbody2D rb;

    private Transform groundCheckUp;
    private Transform groundCheckDown;
    private Transform wallCheck;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;

    private Vector2 attackDirection;
    private Vector3 GroundCheckUpOriginalPosition;
    private Vector3 GroundCheckDownOriginalPosition;

    private float wallCheckRadius = 0.02f;
    private float groundCheckRadius = 0.02f;
    private float groundCheckTimer;

    private bool isFacingRight;
    private bool isTouchingUp;

    private int attackTimes;

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



        isFacingRight = true;
        gruzMother.localScale = new Vector2(-gruzMother.localScale.x, gruzMother.localScale.y);

        if (attackTimes == 0)
            attackTimes = Random.Range(1, 20);
        else if (attackTimes == 1)
        {
            attackTimes = 0;
            animator.Play("GruzMother_Pause");
        }

        attackDirection = new Vector2(1, 4);
        attackDirection.Normalize();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (isTouchingUp)
        {
            attackDirection.y *= -1;
            isTouchingUp = !isTouchingUp;
        }
        if (Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer))
        {

            gruzMother.localScale = new Vector2(-gruzMother.localScale.x, gruzMother.localScale.y);
            isFacingRight = !isFacingRight;
            attackDirection.x *= -1;


        }
        if (Physics2D.OverlapCircle(groundCheckUp.position, groundCheckRadius, groundLayer))
        {

            groundCheckUp.localPosition = Vector3.zero;
            isTouchingUp = true;
            attackTimes--;
            animator.Play("GruzMother_Smashed");

        }
        if (Physics2D.OverlapCircle(groundCheckDown.position, groundCheckRadius, groundLayer))
        {

            groundCheckDown.localPosition = Vector3.zero;
            isTouchingUp = false;
            attackTimes--;
            animator.Play("GruzMother_Smashed");

        }

        rb.velocity = attackDirection * 250 * Time.fixedDeltaTime;


        groundCheckTimer -= Time.deltaTime;

        if (groundCheckTimer <= 0)
        {
            groundCheckUp.localPosition = GroundCheckUpOriginalPosition;
            groundCheckDown.localPosition = GroundCheckDownOriginalPosition;
            groundCheckTimer = 0.5f;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        groundCheckUp.localPosition = GroundCheckUpOriginalPosition;
        groundCheckDown.localPosition = GroundCheckDownOriginalPosition;
        rb.velocity = Vector2.zero;
    }
}
