using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestLifeScript : ChestScript
{
    [Header("Unique")]
    [Space]
    [SerializeField]
    private float lifeToAdd = 50f;

    public override void Collect(PlayerScript playerScript)
    {
        base.Collect(playerScript);

        playerScript.Heal(lifeToAdd);
    }
}
