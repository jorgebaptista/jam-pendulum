using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSwordScript : ChestScript
{
    public override void Collect(PlayerScript playerScript)
    {
        base.Collect(playerScript);

        playerScript.ReceiveSword();
    }
}
