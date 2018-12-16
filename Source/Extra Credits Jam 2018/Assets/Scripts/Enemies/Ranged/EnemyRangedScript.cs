using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedScript : EnemyScript
{
    [Header("Unique Porperties")]
    [Space]
    [Header("Attack")]
    [SerializeField]
    private float shootForce = 300f;
    [SerializeField]
    private float damageForce = 200f;

    [Space]
    [SerializeField]
    private float attackCooldown = 2f;

    [Space]
    [SerializeField]
    private Color alertColor = Color.red;

    [Header("Settings")]
    [Space]
    [SerializeField]
    private Transform shootPoint;
    [SerializeField]
    private GameObject bulletPrefab;

    private int bulletID;

    private float attackTimer;

    private bool isAttacking;

    private PoolManagerScript poolManager;

    protected override void Awake()
    {
        base.Awake();

        mySpriteRenderer = GetComponent<SpriteRenderer>();

        poolManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<PoolManagerScript>();
    }

    protected override void Start()
    {
        base.Start();

        bulletID = poolManager.PreCache(bulletPrefab, 3);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isAlive)
        {
            myAnimator.SetFloat("Speed", Mathf.Round(Mathf.Clamp01(Mathf.Abs(myRigidbody2D.velocity.x))));

            if (isSeeingPlayer)
            {
                canMove = false;
                mySpriteRenderer.color = alertColor;

                if (!isAttacking && Time.time > attackTimer) Attack(true);
            }
            else
            {
                canMove = true;
                mySpriteRenderer.color = initialColor;
            } 
        }
    }

    public void Attack(bool toggle)
    {
        if (toggle)
        {
            isAttacking = true;
            myAnimator.SetTrigger("Attack");
        }
        else
        {
            isAttacking = false;
            attackTimer = attackCooldown + Time.time;
        }
    }

    private void Shoot()
    {
        GameObject bullet = poolManager.GetCachedPrefab(bulletID);

        bullet.transform.SetPositionAndRotation(shootPoint.position, transform.rotation);

        bullet.SetActive(true);
        bullet.GetComponent<EnemyBulletScript>().SetStats(damage, damageForce);
        bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(shootForce * transform.right.x, 0));
    }
}
