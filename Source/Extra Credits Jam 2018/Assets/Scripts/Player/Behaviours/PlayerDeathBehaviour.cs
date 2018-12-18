using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathBehaviour : StateMachineBehaviour
{
    bool done;

    private GameManager gameManager;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gameManager = gameManager ?? GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<GameManager>();

        done = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!done && stateInfo.normalizedTime > .95f)
        {
            done = true;
            gameManager.GameOver();
        }
    }
}
