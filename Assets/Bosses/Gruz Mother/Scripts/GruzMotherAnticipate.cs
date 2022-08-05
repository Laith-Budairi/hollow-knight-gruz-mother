using UnityEngine;

public class GruzMotherAnticipate : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        int nextState = 0;


        if (nextState == 1)
            animator.Play("GruzMother_Charge");

        else
            animator.Play("GruzMother_Attack_Up_Down");

    }
}
