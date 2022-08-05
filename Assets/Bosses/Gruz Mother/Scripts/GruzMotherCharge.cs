using TigerForge;
using UnityEngine;

public class GruzMotherCharge : StateMachineBehaviour
{
    private Transform gruzMother;
    private Rigidbody2D rb;

    private Vector3 playerPosition;

    private const float chargeSpeed = 190;
    private const byte collisionTimer = 1;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isChargingAttack", true);
        //Get components and children of Gruz Mother game object
        gruzMother = GameObject.FindGameObjectWithTag("Boss").GetComponent<Transform>();
        playerPosition = GameObject.FindObjectOfType<Fox>().transform.position;
        rb = gruzMother.GetComponent<Rigidbody2D>();

        Vector3 direction = playerPosition - gruzMother.transform.position;

        rb.velocity = direction.normalized * chargeSpeed * Time.fixedDeltaTime;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
