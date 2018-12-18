using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region Debug
#if UNITY_EDITOR
using UnityEditor;
#endif 
#endregion

public class LampCrusherScript : MonoBehaviour
{
    [SerializeField]
    private float damage = 50f;
    [SerializeField]
    private float damageForce = 500f;

    [Space]
    [SerializeField]
    private bool movingUp;

    [Space]
    [SerializeField]
    private float delay = .15f;
    [SerializeField]
    private float maxSpeedDown = 10f;
    [SerializeField]
    private float maxSpeedUp = 5f;

    [Space]
    [SerializeField]
    private float distanceBot;
    [SerializeField]
    private float distanceTop;

    private Vector2 pointA, pointB;
    private Vector2 velocity = Vector2.zero;

    private bool canDealDamage;

    private void Start()
    {
        pointA = transform.TransformPoint(new Vector2(0, distanceBot));
        pointB = transform.TransformPoint(new Vector2(0, distanceTop));
    }

    private void OnBecameVisible()
    {
        if (!movingUp) StartCoroutine(MoveToA());
        else StartCoroutine(MoveToB());
    }

    private void OnBecameInvisible()
    {
        StopAllCoroutines();
    }

    private IEnumerator MoveToA()
    {
        canDealDamage = true;

        //cheaty
        while (transform.position.y > pointA.y +.2f)
        {
            transform.position = Vector2.SmoothDamp(transform.position,
                pointA, ref velocity, delay, maxSpeedDown, Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }

        transform.position = new Vector2(transform.position.x, pointA.y);

        StartCoroutine(MoveToB());
    }

    private void StartB()
    {
        velocity = Vector2.zero;

        StopAllCoroutines();
        StartCoroutine(MoveToB());
    }

    private IEnumerator MoveToB()
    {
        canDealDamage = false;

        //cheaty
        while (transform.position.y < pointB.y -.2f)
        {
            transform.position = Vector2.SmoothDamp(transform.position,
                pointB, ref velocity, delay, maxSpeedUp, Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }

        transform.position = new Vector2(transform.position.x, pointB.y);

        StartCoroutine(MoveToA());
    }

    private void StartA()
    {
        velocity = Vector2.zero;

        StopAllCoroutines();
        StartCoroutine(MoveToA());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canDealDamage) collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, damage, damageForce);
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
            transform.TransformPoint(new Vector2(0, distanceBot)),
            transform.TransformPoint(new Vector2(0, distanceTop))
        };

        Handles.color = linesColor;
        Handles.DrawDottedLines(debugLimits, dottedLinesDensity);

        Gizmos.color = linesColor;
        Gizmos.DrawSphere(transform.TransformPoint(new Vector2(0, distanceBot)), .3f);
        Gizmos.DrawSphere(transform.TransformPoint(new Vector2(0, distanceTop)), .3f);
    }
#endif 
    #endregion
}
