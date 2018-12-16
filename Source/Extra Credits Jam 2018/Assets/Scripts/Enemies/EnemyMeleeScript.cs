using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeScript : EnemyScript
{
    [Header("Unique Properties")]
    [Space]

    [Header("Attack")]
    [Space]
    [SerializeField]
    [Tooltip("Movement speed increase when enemy is seeing player. 1 for no change")]
    private float hostileMoveSpeedMultiplier = 2f;

    [Space]
    [SerializeField]
    [Tooltip("Time in seconds each time player is damaged. Coordinate with Damage variable.")]
    private float damageTimer = 0.2f;
    [SerializeField]
    private float damageForce = 50f;

    [Space]
    [SerializeField]
    private Color hostileLightColor = Color.red;

    [Header("Settings")]
    [Space]
    [SerializeField]
    private SpriteRenderer spotLight;

    private Color initialLightColor;

    private PlayerScript playerScript;

    protected override void Start()
    {
        base.Start();

        initialLightColor = spotLight.color;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isAlive)
        {
            myAnimator.SetFloat("Speed", currentMoveSpeed / moveSpeed);

            if (isSeeingPlayer)
            {
                currentMoveSpeed = moveSpeed * hostileMoveSpeedMultiplier;
                spotLight.color = hostileLightColor;
            }
            else
            {
                currentMoveSpeed = moveSpeed;
                spotLight.color = initialLightColor;
            } 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerScript = playerScript ?? collision.GetComponent<PlayerScript>();
            StopAllCoroutines();
            StartCoroutine(DamagePlayer());
        }
        else if (collision.CompareTag("PlayerDummy"))
        {
            playerScript = playerScript ?? GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
            StopAllCoroutines();
            StartCoroutine(DamagePlayer(true));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        StopAllCoroutines();
    }

    private IEnumerator DamagePlayer(bool isDummy = false)
    {
        while(true)
        {
            yield return new WaitForSeconds(damageTimer);

            if (!isDummy) playerScript.TakeDamage(damage, damageForce, transform);
            else playerScript.TakeDamage(damage, damageForce, transform, true);
        }
    }
}
