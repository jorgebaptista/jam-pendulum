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

    [Header("Audio")]
    [Space]
    [SerializeField]
    private string mainMusicSound = "Main_Music";
    [SerializeField]
    private string gameOverSound = "GameOver";

    private bool hasChecked;

    private Vector2 lastCheckpointPos;

    private PlayerScript playerScript;
    private AudioManagerScript audioManager;

    private void Awake()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        audioManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<AudioManagerScript>();
    }

    private void Start()
    {
        //audioManager.PlaySound(mainMusicSound, name);
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
        audioManager.PlaySound(gameOverSound, name);

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