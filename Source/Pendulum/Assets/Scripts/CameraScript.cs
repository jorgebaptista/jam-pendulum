using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region Debug
#if UNITY_EDITOR
using UnityEditor;
#endif 
#endregion

public class CameraScript : MonoBehaviour
{
    [Header("Smooth Scroll")]
    [Space]
    [SerializeField]
    private float delay = 0.1f;

    [Space]
    [SerializeField]
    private float playerOffset = 1f;

    private Transform playerTransform;

    [Header("Limits")]
    [Space]
    [SerializeField]
    [Tooltip("Limit camera movement? (ej. not allowing camera to go outside boundaries of map) " +
        "Must set limits according to map size, use the magenta rectangle in the editor as a guiding.")]
    private bool useLimits;

    [SerializeField]
    private Vector2 minLimit;
    [SerializeField]
    private Vector2 maxLimit;

    private Vector2 currentMaxLimit, currentMinLimit;

    private float minLimitX, maxLimitX, minLimitY, maxLimitY;
    private float horizontalExtent, verticalExtent;

    private Vector3 targetPosition, velocity, offset, lastOffsetPosition;

    public void SetupRealLimits(Camera camera, Vector2 max, Vector2 min)
    {
        currentMaxLimit = max;
        currentMinLimit = min;

        if (camera.orthographic)
        {
            verticalExtent = camera.orthographicSize;
            horizontalExtent = verticalExtent * ((float)Screen.width / Screen.height);

            minLimitX = min.x + horizontalExtent;
            maxLimitX = max.x - horizontalExtent;

            minLimitY = min.y + verticalExtent;
            maxLimitY = max.y - verticalExtent;
            return;
        }

        Debug.LogError("You might want to change camera to Orthographic.");
    }

    public bool IsOutsideCameraX(float positionX)
    {
        return positionX > currentMaxLimit.x || positionX < currentMinLimit.x;
    }

    public bool IsOutsideCameraY(float positionY)
    {
        return positionY < currentMinLimit.y;
    }

    private void Awake()
    {
        currentMaxLimit = maxLimit;
        currentMinLimit = minLimit;

        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        SetupRealLimits(GetComponent<Camera>(), maxLimit, minLimit);
        offset = transform.position;
    }

    private void LateUpdate()
    {
        targetPosition = playerTransform.position;

        targetPosition.y += offset.y;
        targetPosition.z += offset.z;

        if (useLimits)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x,
                minLimitX, maxLimitX);
            targetPosition.y = Mathf.Clamp(targetPosition.y,
                minLimitY, maxLimitY);
        }

        transform.position = Vector3.SmoothDamp(transform.position,
            new Vector3(targetPosition.x, targetPosition.y + playerOffset, targetPosition.z), ref velocity, delay);
    }

    #region Shake Camera
    public void ShakeCamera(float range = 2, float duration = 0.5f)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(duration, range));
    }

    private IEnumerator DoShake(float duration, float range)
    {
        while (duration > 0)
        {
            transform.localPosition -= lastOffsetPosition;

            lastOffsetPosition = Random.insideUnitCircle * range;

            transform.localPosition += lastOffsetPosition;

            if (duration < 0.5f)
            {
                range *= 0.9f;
            }

            duration -= Time.deltaTime;
            yield return null;
        }
    } 
    #endregion

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
        if (useLimits)
        {
            Vector3[] debugLimits = {
                new Vector2(minLimit.x, minLimit.y),
                new Vector2(minLimit.x, maxLimit.y),

                new Vector2(maxLimit.x, maxLimit.y),
                new Vector2(maxLimit.x, minLimit.y),

                new Vector2(minLimit.x, minLimit.y),
                new Vector2(maxLimit.x, minLimit.y),

                new Vector2(maxLimit.x, maxLimit.y),
                new Vector2(minLimit.x, maxLimit.y)
            };

            Handles.color = linesColor;
            Handles.DrawDottedLines(debugLimits, dottedLinesDensity);
        }
    }
#endif 
    #endregion
}
