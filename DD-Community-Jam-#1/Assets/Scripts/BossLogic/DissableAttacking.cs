using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissableAttacking : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInParent<RobotBoss>().isAttacking = false;
    }
}
