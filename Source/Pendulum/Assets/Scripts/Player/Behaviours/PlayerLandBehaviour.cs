using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandBehaviour : StateMachineBehaviour
{
    PlayerScript playerScript;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerScript = playerScript ?? animator.GetComponent<PlayerScript>();
        playerScript.PlayLandSound();
    }
}
