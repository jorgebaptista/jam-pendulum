using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackEndBehaviour : StateMachineBehaviour
{
    private PlayerScript playerScript;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerScript = playerScript ?? animator.GetComponent<PlayerScript>();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerScript.EndAttack();
    }
}
