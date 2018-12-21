using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour, IActivable
{
    public void Activate()
    {
        gameObject.SetActive(false);
    }
}
