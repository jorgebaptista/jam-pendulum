using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region Debug
#if UNITY_EDITOR
using UnityEditor;
#endif 
#endregion

public class CheckpointScript : MonoBehaviour
{
    [Header("Properties")]
    [Space]
    [SerializeField]
    private Sprite activeSprite;

    [Space]
    [SerializeField]
    private Transform spawner;

    [Space]
    [SerializeField]
    private bool changeCameraLimits;
    [SerializeField]
    private Vector2 minLimit;
    [SerializeField]
    private Vector2 maxLimit;

    [Header("Audio")]
    [Space]
    [SerializeField]
    private string checkpointSound = "Checkpoint";

    private SpriteRenderer mySpriteRenderer;
    private Collider2D myCollider2D;

    private CameraScript cameraScript;
    private GameManager gameManager;
    private AudioManagerScript audioManager;

    private void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myCollider2D = GetComponent<Collider2D>();

        cameraScript = Camera.main.GetComponent<CameraScript>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<GameManager>();
        audioManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<AudioManagerScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameManager.LastCheckpointPos = spawner.position;

            mySpriteRenderer.sprite = activeSprite;
            myCollider2D.enabled = false;

            if (changeCameraLimits) cameraScript.SetupRealLimits(cameraScript.GetComponent<Camera>(), maxLimit, minLimit);

            audioManager.PlaySound(checkpointSound, name);
        }
    }

    #region Debug
#if UNITY_EDITOR
    [Header("Debug")]
    [Space]
    [SerializeField]
    [Tooltip("Settings for the debug rectangle that guide camera limits")]
    float dottedLinesDensity = 5f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(spawner.position, .5f);

        if (changeCameraLimits)
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

            Handles.color = Color.magenta;
            Handles.DrawDottedLines(debugLimits, dottedLinesDensity);
        }
    }
#endif 
    #endregion
}
