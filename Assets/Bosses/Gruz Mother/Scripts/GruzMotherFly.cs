using UnityEngine;

public class GruzMotherFly : StateMachineBehaviour
{
    [SerializeField] GruzMother gruzMother;
    private float stateTimer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Get script and set initial state
        stateTimer = 3;

        if(gruzMother == null)
            gruzMother = GameObject.FindGameObjectWithTag("Boss").GetComponent<GruzMother>();
        
        gruzMother.FlyStartState();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If timer finishes then move to the next state
        if (stateTimer > 0)
        {
            stateTimer -= Time.deltaTime;
            gruzMother.Fly();
        }
        else
        {
            animator.Play("GruzMother_Anticipation");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gruzMother.FlyExitState();
    }

}
