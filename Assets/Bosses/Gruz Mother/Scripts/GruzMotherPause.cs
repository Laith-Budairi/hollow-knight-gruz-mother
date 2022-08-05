using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruzMotherPause : StateMachineBehaviour
{
    private float stateTimer = 1;

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateTimer > 0)
            stateTimer -= Time.deltaTime;
        else
            animator.Play("GruzMother_Fly");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        stateTimer = 1;
    }


}
