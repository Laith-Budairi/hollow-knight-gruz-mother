using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruzMotherPause : StateMachineBehaviour
{
    [SerializeField] GruzMother gruzMother;

    private float stateTimer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Get components and children of Gruz Mother game object
        if (gruzMother == null)
            gruzMother = GameObject.FindGameObjectWithTag("Boss").GetComponent<GruzMother>();

        stateTimer = 1;

        gruzMother.PauseStartState();

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateTimer > 0)
            stateTimer -= Time.deltaTime;
        else
            animator.Play("GruzMother_Fly");
    }
}
