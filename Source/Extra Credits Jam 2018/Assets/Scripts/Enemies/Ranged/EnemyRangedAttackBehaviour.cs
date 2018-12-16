using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttackBehaviour : StateMachineBehaviour
{
    EnemyRangedScript enemyRangedScript;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyRangedScript = enemyRangedScript ?? animator.GetComponent<EnemyRangedScript>();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyRangedScript.Attack(false);
    }
}
