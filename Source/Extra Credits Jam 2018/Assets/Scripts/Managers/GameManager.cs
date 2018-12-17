using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region Debug
#if UNITY_EDITOR
using UnityEditor;
#endif 
#endregion

public class GameManager : MonoBehaviour
{
    [Header("Respawn")]
    [Space]
    [SerializeField]
    private float respawnTimer = 2f;
    [SerializeField]
    private Vector2 initialCheckpoint;

    private bool hasChecked;

    private Vector2 lastCheckpointPos;

    private PlayerScript playerScript;

    private void Awake()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    public Vector2 LastCheckpointPos
    {
        set
        {
            lastCheckpointPos = value;
            if (!hasChecked) hasChecked = true;
        }
    }

    public void GameOver()
    {
        StopAllCoroutines();
        StartCoroutine(RespawnPlayer());
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(respawnTimer);

        playerScript.transform.position = hasChecked ? lastCheckpointPos : initialCheckpoint;
        playerScript.Revive();
    }

    #region Debug
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(initialCheckpoint, .5f);
    }
#endif 
    #endregion
}