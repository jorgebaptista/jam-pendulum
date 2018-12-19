using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region Debug
#if UNITY_EDITOR
using UnityEditor;
#endif 
#endregion

public class MovingPlatformScript : MonoBehaviour, IActivable
{
    [Header("Properties")]
    [Space]
    [SerializeField]
    private bool moveOnce;

    [Space]
    [SerializeField]
    private Vector2 pointA;
    [SerializeField]
    private Vector2 pointB;

    [Space]
    [SerializeField]
    private float smooth = 3f;
    [SerializeField]
    private float maxSpeed = 5f;

    [Space]
    [SerializeField]
    private bool active;
    [SerializeField]
    private bool movingA;

    private Vector2 velocity = Vector2.zero;

    private Vector2 realPointA;
    private Vector2 realPointB;

    private void Start()
    {
        realPointA = transform.TransformPoint(pointA);
        realPointB = transform.TransformPoint(pointB);

        if (active)
        {
            if (movingA) StartCoroutine(MoveToA());
            else StartCoroutine(MoveToB());
        }
    }

    public void Activate()
    {
        StopAllCoroutines();

        if (moveOnce)
        {
            if (movingA)
            {
                StartCoroutine(MoveToA());
                movingA = false;
            }
            else
            {
                StartCoroutine(MoveToB());
                movingA = true;
            }
        }
        else
        {
            active = !active;

            if (active)
            {
                if (movingA) StartCoroutine(MoveToA());
                else StartCoroutine(MoveToB());
            }
        }
    }

    private IEnumerator MoveToA()
    {
        if (!moveOnce) movingA = true;

        //cheaty
        while (transform.position.x > realPointA.x + 1f || transform.position.y > realPointA.y + 1f)
        {
            transform.position = Vector2.SmoothDamp(transform.position,
                realPointA, ref velocity, smooth, maxSpeed, Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }

        if (!moveOnce) StartCoroutine(MoveToB());
    }

    private IEnumerator MoveToB()
    {
        if (!moveOnce) movingA = false;

        //cheaty
        while (transform.position.x < realPointB.x - 1f || transform.position.y < realPointB.y - 1f)
        {
            transform.position = Vector2.SmoothDamp(transform.position,
                realPointB, ref velocity, smooth, maxSpeed, Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }

        if (!moveOnce) StartCoroutine(MoveToA());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = transform;
            collision.attachedRigidbody.interpolation = RigidbodyInterpolation2D.None;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = null;
            collision.attachedRigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

    #region Debug
#if UNITY_EDITOR

    [Header("Debug")]
    [Space]
    [SerializeField]
    [Tooltip("Settings for the debug rectangle that guide camera limits")]
    float dottedLinesDensity = 5f;

    [SerializeField]
    private Color linesColor = Color.green;

    private void OnDrawGizmosSelected()
    {
        Vector3[] debugLimits = {
            transform.TransformPoint(pointA),
            transform.TransformPoint(pointB)
    };

        Handles.color = linesColor;
        Handles.DrawDottedLines(debugLimits, dottedLinesDensity);

        Gizmos.color = linesColor;
        Gizmos.DrawSphere(transform.TransformPoint(pointA), .3f);
        Gizmos.DrawSphere(transform.TransformPoint(pointB), .3f);
    }
#endif 
    #endregion
}
