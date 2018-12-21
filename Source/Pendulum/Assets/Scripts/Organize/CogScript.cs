using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogScript : MonoBehaviour, IActivable
{
    [SerializeField]
    private bool rotateRight;

    [SerializeField]
    private float speed = 3f;

    private bool canRotate = true;

    private Animator myAnimator;

    private void FixedUpdate()
    {
        if (canRotate) transform.Rotate(new Vector3(0, 0, rotateRight? speed : -speed));
    }

    public void Activate()
    {
        //canRotate = false;

        rotateRight = !rotateRight;
    }
}
