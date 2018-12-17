using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    private float damage, damageToFutureSelf;
    private float damageForce;

    [SerializeField]
    private string hitSound = "Enemy_Bullet_Hit";

    private TrailRenderer myTrailRenderer;
    private AudioSource myAudioSource;

    private PlayerScript playerScript;
    private AudioManagerScript audioManager;

    private void Awake()
    {
        myTrailRenderer = GetComponent<TrailRenderer>();
        myAudioSource = GetComponent<AudioSource>();

        audioManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<AudioManagerScript>();
    }

    private void OnEnable()
    {
        myTrailRenderer.enabled = true;
        myTrailRenderer.emitting = true;
    }

    private void OnDisable()
    {
        myTrailRenderer.emitting = false;
        myTrailRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerScript = playerScript ?? collision.GetComponent<PlayerScript>();

            playerScript.TakeDamage(damage, damageToFutureSelf, damageForce, transform);

            audioManager.PlaySound(hitSound, gameObject.name, myAudioSource);
        }
        else if (collision.CompareTag("PlayerDummy"))
        {
            playerScript = playerScript ?? GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();

            playerScript.TakeDamage(damage, damageToFutureSelf, damageForce, transform, true);

            audioManager.PlaySound(hitSound, gameObject.name, myAudioSource);
        }

        gameObject.SetActive(false);
    }

    public void SetStats(float damageAmount, float damageToFS, float damageForceAmount, string bulletHitSound)
    {
        damage = damageAmount;
        damageToFutureSelf = damageToFS;
        damageForce = damageForceAmount;

        if (hitSound == null) hitSound = bulletHitSound;
    }
}
