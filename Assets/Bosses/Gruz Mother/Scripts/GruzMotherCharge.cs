using UnityEngine;

public class GruzMotherCharge : StateMachineBehaviour
{
    [SerializeField] GruzMother gruzMother;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (gruzMother == null)
            gruzMother = GameObject.FindGameObjectWithTag("Boss").GetComponent<GruzMother>();

        gruzMother.ChargeAttack();      
    }
}
