using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region Debug
#if UNITY_EDITOR
using UnityEditor;
#endif 
#endregion

public class PlayerScript : MonoBehaviour
{
    #region Variables
    [Header("Life")]
    [Space]
    [SerializeField]
    private float life = 150f;

    [Header("Movement")]
    [Space]
    [SerializeField]
    private float moveSpeed = 3f;
    [SerializeField]
    private bool facingRight = true;

    [Space]
    [SerializeField]
    private float jumpForce = 250f;

    [Header("Attack")]
    [Space]
    [SerializeField]
    private float attackDashForce = 500f;

    [Space]
    [SerializeField]
    private float attackShakeForce = .5f;
    [SerializeField]
    private float attackShakeDuration = .5f;
    [SerializeField]
    private float attackFreezeDuration = .3f;

    [Header("Future Ability")]
    [Space]
    [SerializeField]
    private float transitDashForce = 500f;

    [Space]
    [SerializeField]
    [Tooltip("Duration in seconds")]
    private float futureSelfDuration = 10f;

    [Space]
    [SerializeField]
    private Color futureSelfColor;
    [SerializeField]
    [Tooltip("Time until after image fades.")]
    private float afterImageLifeTime = .3f;
    [SerializeField]
    private float distancePerAfterImage = .5f;

    [Header("Settings")]
    [Space]
    [SerializeField]
    private float dashDrag = 6f;
    [SerializeField]
    private float dashEndCap = 4f;

    [Space]
    [SerializeField]
    private GameObject jumpEffectPrefab;

    [Space]
    [SerializeField]
    private Transform feet;
    [SerializeField]
    private float groundCheckRadius = 0.2f;
    [SerializeField]
    private LayerMask groundLayerMask;

    [Space]
    [SerializeField]
    private float interactTriggerRadius = 1f;
    [SerializeField]
    private LayerMask interactableLayer;

    [Space]
    [SerializeField]
    private BoxCollider2D attackTrigger;

    [Space]
    [SerializeField]
    private GameObject playerDummyPrefab;
    [SerializeField]
    [Tooltip("Use number of layermask, couldn't find out how to choose layer directly")]
    private int futureSelfLayerMask = 15;

    [Space]
    [SerializeField]
    private RuntimeAnimatorController animatorWithWeapon;

    [Header("Audio")]
    [Space]
    [SerializeField]
    private string footstepsSound = "Player_Footsteps";
    [SerializeField]
    private string jumpSound = "Player_Jump";
    [SerializeField]
    private string landSound = "Player_Land";
    [SerializeField]
    private string attackSound = "Player_Attack";
    [SerializeField]
    private string futureDashSound = "Player_FutureDash";
    [SerializeField]
    private string hurtSound = "Player_Hurt";
    [SerializeField]
    private string deathSound = "Player_Death";
    [SerializeField]
    private string healSound = "Player_Heal";
    [SerializeField]
    private string receiveSwordSound = "Player_Collect_Sword";

    [Space]
    [SerializeField]
    private string enemyHitSound = "Enemy_Hit";

    private float currentLife;
    private float horizontal;
    private float futureSelfTimer;

    private int jumpEffectID, playerDummyID, afterImageID;

    private bool isAlive;
    private bool isGrounded, isDashing;
    private bool interacting;
    private bool isArmed, isAttacking;
    private bool isTransiting;
    private bool dummyIsFacingRight;

    private GameObject playerDummy;

    private LayerMask initialLayerMask;

    private Vector2 playerDummyPosition;
    private Vector2 lastAfterImagePos;

    private Color initialColor;

    private SpriteRenderer mySpriteRenderer;
    private Animator myAnimator;
    private Rigidbody2D myRigidbody2D;
    private Collider2D myCollider2D;

    private GameManager gameManager;
    private UIManagerScript uIManager;
    private AudioManagerScript audioManager;
    private PoolManagerScript poolManager;
    private CameraScript cameraScript;
    #endregion

    #region Main
    private void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myCollider2D = GetComponent<Collider2D>();

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<GameManager>();
        uIManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<UIManagerScript>();
        audioManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<AudioManagerScript>();
        poolManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<PoolManagerScript>();
        cameraScript = Camera.main.GetComponent<CameraScript>();
    }

    private void Start()
    {
        Revive();

        initialLayerMask = gameObject.layer;

        myRigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;

        uIManager.ToggleTimerBar(false);

        jumpEffectID = poolManager.PreCache(jumpEffectPrefab, 1);
        playerDummyID = poolManager.PreCache(playerDummyPrefab, 1);
        afterImageID = poolManager.PreCache(new GameObject("After Image", typeof(SpriteRenderer)));
    }

    private void Update()
    {
        if (isAlive && !interacting)
        {
            horizontal = Input.GetAxisRaw("Horizontal");

            if (!isDashing)
            {
                if (!isAttacking)
                {
                    if (isGrounded)
                    {
                        if (Input.GetButtonDown("Jump")) myAnimator.SetTrigger("Jump");
                        if (Input.GetButtonDown("Interact")) CheckInteract();
                    }
                    if (!isTransiting && isArmed && Input.GetButtonDown("Attack"))
                    {
                        isAttacking = true;
                        myAnimator.SetTrigger("Attack");
                    } 
                }

                if (Input.GetButtonDown("Transit"))
                {
                    if (!isTransiting && isGrounded && !isAttacking) TransitToFuture();
                    else if (isTransiting) TransitToPresent();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(feet.position, groundCheckRadius, groundLayerMask);

        myAnimator.SetFloat("Horizontal", Mathf.Abs(horizontal));
        myAnimator.SetFloat("Vertical", myRigidbody2D.velocity.y);
        myAnimator.SetBool("Grounded", isGrounded);
        myAnimator.SetBool("Dashing", isDashing);

        if (isAlive && !isGrounded && myRigidbody2D.velocity.y > -Physics2D.gravity.y) myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, -Physics2D.gravity.y);

        if (!isDashing && !isAttacking)
        {
            myRigidbody2D.velocity = new Vector2(!cameraScript.IsOutsideCameraX(transform.position.x + (transform.right.x * .5f)) ? horizontal * moveSpeed : 0,
              myRigidbody2D.velocity.y);

            if (facingRight && horizontal < 0 || !facingRight && horizontal > 0) Flip();
        }

        if (isTransiting)
        {
            float distanceFromLastImage = Vector2.Distance(lastAfterImagePos, transform.position);

            if (distanceFromLastImage > distancePerAfterImage)
            {
                SpriteRenderer afterImageSprite = poolManager.GetCachedPrefab(afterImageID).GetComponent<SpriteRenderer>();

                afterImageSprite.sprite = mySpriteRenderer.sprite;
                afterImageSprite.sortingLayerID = mySpriteRenderer.sortingLayerID;
                afterImageSprite.sortingOrder = mySpriteRenderer.sortingOrder - 1;

                afterImageSprite.transform.SetPositionAndRotation(transform.position, transform.rotation);
                afterImageSprite.transform.localScale = mySpriteRenderer.transform.lossyScale;

                afterImageSprite.gameObject.SetActive(true);

                StartCoroutine(FadeAfterImage(afterImageSprite));

                lastAfterImagePos = transform.position;
                distanceFromLastImage = 0;
            }

            if (Time.time > futureSelfTimer) TransitToPresent();
            else uIManager.UpdateTimerBar(Mathf.Clamp01((futureSelfTimer - Time.time) / futureSelfDuration));
        }

        if (isAlive && cameraScript.IsOutsideCameraY(transform.position.y))
        {
            if (!isTransiting)
            {
                isAlive = false;
                gameManager.GameOver();
            }
            else TransitToPresent();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) StartCoroutine(DealDamage(collision.GetComponent<EnemyScript>()));
    }
    #endregion

    #region Life
    public void Revive()
    {
        isAlive = true;
        currentLife = life;

        uIManager.UpdateLifeBar(currentLife / life);

        attackTrigger.enabled = false;

        myAnimator.SetTrigger("Reset");
        myRigidbody2D.isKinematic = false;
        myCollider2D.enabled = true;
    }

    public void TakeDamage(float damageAmount, float damageToFS,float damageForce = 150f, Transform damager = null, bool isDummy = false)
    {
        EndAttack();

        if (isAlive)
        {
            if (isTransiting && !isDummy) futureSelfTimer -= damageToFS;
            else
            {
                if (isDummy) TransitToPresent();

                currentLife -= damageAmount;

                if (currentLife < 0) currentLife = 0;

                uIManager.UpdateLifeBar(currentLife / life);

                if (currentLife == 0)
                {
                    StartCoroutine(Die());
                    return;
                }
            }
            myAnimator.SetTrigger("Hurt");
            audioManager.PlaySound(hurtSound, name);

            if (damager && ((damager.position.x > transform.position.x && !facingRight) || (damager.position.x < transform.position.x && facingRight))) Flip(false);

            StopCoroutine("Dash");
            StartCoroutine("Dash", -damageForce * transform.right.x); 
        }
    }

    private IEnumerator Die()
    {
        isAlive = false;

        horizontal = 0;

        if (!isGrounded)
        {
            myAnimator.SetTrigger("Hurt");
            audioManager.PlaySound(hurtSound, name);
        }
        yield return new WaitUntil(() => isGrounded);
        myAnimator.SetTrigger("Die");
        audioManager.PlaySound(deathSound, name);

        myRigidbody2D.isKinematic = true;
        myRigidbody2D.velocity = Vector2.zero;

        myCollider2D.enabled = false;
    }
    #endregion

    #region Movement
    private void Flip(bool playAnimation = true)
    {
        Vector2 currentEulerAngles = transform.eulerAngles;
        currentEulerAngles.y += 180;
        transform.eulerAngles = currentEulerAngles;

        if (isGrounded && playAnimation) myAnimator.SetTrigger("Turn");

        facingRight = !facingRight;
    }

    private void Jump()
    {
        myRigidbody2D.AddForce(new Vector2(0, jumpForce));

        GameObject jumpEffect = poolManager.GetCachedPrefab(jumpEffectID);
        jumpEffect.transform.position = feet.position;
        jumpEffect.SetActive(true);

        audioManager.PlaySound(jumpSound, name);
    }

    private IEnumerator Dash(float force)
    {
        isDashing = true;

        myRigidbody2D.drag = dashDrag;
        myRigidbody2D.velocity = Vector2.zero;
        myRigidbody2D.AddForce(new Vector2(force, 0));

        yield return new WaitForSeconds(.1f);
        yield return new WaitUntil(() => Mathf.Abs(myRigidbody2D.velocity.x) < dashEndCap);

        isDashing = false;
        myRigidbody2D.drag = 0;

    }
    #endregion

    #region Interact
    private void CheckInteract()
    {
        interacting = true;

        if (Physics2D.OverlapCircle(transform.position, interactTriggerRadius, interactableLayer))
        {
            InteractableScript interactableScript = Physics2D.OverlapCircle(transform.position, interactTriggerRadius, interactableLayer).GetComponent<InteractableScript>();

            if (!isTransiting || (isTransiting && interactableScript.CanFSInteract))
            {
                if (interactableScript.CanCollect) myAnimator.SetTrigger("Pickup");
                else myAnimator.SetTrigger("Interact");
                return;
            }
            else interacting = false;
        }

        interacting = false;
    }

    private void Interact()
    {
        InteractableScript interactableScript = Physics2D.OverlapCircle(transform.position, interactTriggerRadius, interactableLayer).GetComponent<InteractableScript>();

        if (interactableScript)
        {
            interactableScript.Interact();
        }
        else Debug.LogError("InteractableScript not found.");

        interacting = false;
    }

    private void Collect()
    {
        ChestScript chestScript = Physics2D.OverlapCircle(transform.position, interactTriggerRadius, interactableLayer).GetComponent<ChestScript>();

        if (chestScript)
        {
            chestScript.Collect(this);
        }
        else Debug.LogError("ChestScript not found.");
    }

    public void ReceiveSword()
    {
        myAnimator.runtimeAnimatorController = animatorWithWeapon;

        audioManager.PlaySound(receiveSwordSound, name);

        interacting = false;
        isArmed = true;
    }

    public void Heal(float lifeToAdd)
    {
        currentLife += lifeToAdd;
        if (currentLife > life) currentLife = life;
        uIManager.UpdateLifeBar(currentLife / life);

        audioManager.PlaySound(healSound, name);

        interacting = false;
    }
    #endregion

    #region Attack
    private void Attack()
    {
        StartCoroutine(Dash(attackDashForce * transform.right.x));
        attackTrigger.enabled = true;

        audioManager.PlaySound(attackSound, name);
    }

    public void EndAttack()
    {
        attackTrigger.enabled = false;
        isAttacking = false;
    }

    private IEnumerator DealDamage(EnemyScript enemyScript)
    {
        audioManager.PlaySound(enemyHitSound, name);

        cameraScript.ShakeCamera(attackShakeForce, attackShakeDuration);

        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(attackFreezeDuration);
        Time.timeScale = 1;

        enemyScript.TakeDamage();
    }
    #endregion

    #region Future Self
    private void TransitToFuture()
    {
        playerDummy = poolManager.GetCachedPrefab(playerDummyID);
        playerDummy.transform.SetPositionAndRotation(transform.position, transform.rotation);
        playerDummy.GetComponent<SpriteRenderer>().sprite = mySpriteRenderer.sprite;
        dummyIsFacingRight = facingRight;
        playerDummy.SetActive(true);

        futureSelfTimer = Time.time + futureSelfDuration;
        uIManager.ToggleTimerBar(true);

        isTransiting = true;

        initialColor = mySpriteRenderer.color;
        mySpriteRenderer.color = futureSelfColor;
        gameObject.layer = futureSelfLayerMask;

        StartCoroutine(Dash(transitDashForce * transform.right.x));

        audioManager.PlaySound(futureDashSound, name);
    }

    private void TransitToPresent()
    {
        isTransiting = false;

        uIManager.ToggleTimerBar(false);
        uIManager.UpdateTimerBar(1);

        myAnimator.SetTrigger("Reset");
        mySpriteRenderer.color = initialColor;
        gameObject.layer = initialLayerMask;
        facingRight = dummyIsFacingRight;
        attackTrigger.enabled = false;

        transform.SetPositionAndRotation(playerDummy.transform.position, playerDummy.transform.rotation);
        myRigidbody2D.velocity = Vector2.zero;

        playerDummy.SetActive(false);
    }

    private IEnumerator FadeAfterImage(SpriteRenderer afterImageSprite)
    {
        float timeElapsed = 0;

        while (timeElapsed < afterImageLifeTime)
        {
            afterImageSprite.color = Color.Lerp(mySpriteRenderer.color, Color.clear, Mathf.Clamp01(timeElapsed / afterImageLifeTime));

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        afterImageSprite.gameObject.SetActive(false);
        afterImageSprite.color = mySpriteRenderer.color;
    }
    #endregion

    #region Audio
    private void PlayFootstepSound()
    {
        audioManager.PlaySound(footstepsSound, name);
    } 

    public void PlayLandSound()
    {
        audioManager.PlaySound(landSound, name);
    }
    #endregion

    #region Debug
#if UNITY_EDITOR
    [Header("Debug")]
    [Space]
    [SerializeField]
    private bool showGroundCheck = true;
    [SerializeField]
    private bool showInteractCheck = true;
    private void OnDrawGizmosSelected()
    {
        if (showGroundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(feet.position, groundCheckRadius); 
        }
        if(showInteractCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, interactTriggerRadius);
        }
    }
#endif 
    #endregion
}
