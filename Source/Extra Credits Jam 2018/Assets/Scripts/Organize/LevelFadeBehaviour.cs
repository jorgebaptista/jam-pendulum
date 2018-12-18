using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFadeBehaviour : StateMachineBehaviour
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime > .99f) animator.gameObject.SetActive(false);
    }
}
