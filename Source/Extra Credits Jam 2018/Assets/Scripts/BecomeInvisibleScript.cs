using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BecomeInvisibleScript : MonoBehaviour
{
    private bool active;

    private void OnBecameVisible()
    {
        active = true;
        GetComponent<Animator>().enabled = true;
    }

    private void OnBecameInvisible()
    {
        if (active) gameObject.SetActive(false);
    }
}
