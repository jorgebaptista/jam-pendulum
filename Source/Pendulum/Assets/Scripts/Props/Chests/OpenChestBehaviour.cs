using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChestBehaviour : StateMachineBehaviour
{
    private ChestScript chestScript;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        chestScript = animator.GetComponent<ChestScript>();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        chestScript.TurnCollectible();
    }
}
