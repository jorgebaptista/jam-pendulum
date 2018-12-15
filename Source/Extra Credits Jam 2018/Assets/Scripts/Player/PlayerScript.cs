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

    [Space]
    [SerializeField]
    private float dashForce = 500;

    [Header("Attack")]
    [Space]
    [SerializeField]
    private float damage = 50f;
    [SerializeField]
    private float attackForce = 500;

    [Header("Future Ability")]
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
    private BoxCollider2D attackTrigger;

    [Space]
    [SerializeField]
    private GameObject playerDummyPrefab;

    private float horizontal;
    private float gravityValue;
    private float futureSelfTimer;

    private int jumpEffectID, playerDummyID, afterImageID;

    private bool isAlive;
    private bool isGrounded, isDashing;
    private bool isAttacking;
    private bool isTransiting;
    private bool dummyIsFacingRight;

    private GameObject playerDummy;

    private Vector2 originalGravity;
    private Vector2 playerDummyPosition;
    private Vector2 lastAfterImagePos;

    private Color initialColor;

    private SpriteRenderer mySpriteRenderer;
    private Animator myAnimator;
    private Rigidbody2D myRigidbody2D;

    private CameraScript cameraScript;
    private PoolManagerScript poolManager;
    #endregion

    #region Main
    private void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        myRigidbody2D = GetComponent<Rigidbody2D>();

        cameraScript = Camera.main.GetComponent<CameraScript>();
        poolManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<PoolManagerScript>();
    }

    private void Start()
    {
        isAlive = true;

        originalGravity = Physics2D.gravity;
        gravityValue = originalGravity.y;

        attackTrigger.enabled = false;

        jumpEffectID = poolManager.PreCache(jumpEffectPrefab, 1);
        playerDummyID = poolManager.PreCache(playerDummyPrefab, 1);
        afterImageID = poolManager.PreCache(new GameObject("After Image", typeof(SpriteRenderer)));
    }

    private void Update()
    {
        if (isAlive)
        {
            horizontal = Input.GetAxisRaw("Horizontal");

            if (!isDashing && isGrounded && !isAttacking)
            {
                if (Input.GetButtonDown("Jump")) myAnimator.SetTrigger("Jump");
                if (Input.GetButtonDown("Transit"))
                {
                    if (!isTransiting) TransitToFuture();
                    else TransitToPresent();
                }
                if (Input.GetButtonDown("Attack"))
                {
                    isAttacking = true;
                    myAnimator.SetTrigger("Attack");
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

        if (!isDashing && !isAttacking)
        {
            myRigidbody2D.velocity = new Vector2(!cameraScript.IsOutsideCameraX(transform.position.x + (transform.right.x * .5f)) ? horizontal * moveSpeed : 0,
              myRigidbody2D.velocity.y);

            if (facingRight && horizontal < 0 || !facingRight && horizontal > 0) Flip();
        }
        else if (myRigidbody2D.velocity.x * transform.right.x < 0)
        {
            isDashing = false;
            Physics2D.gravity = originalGravity;
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
        }
    } 
    #endregion

    #region Movement
    private void Flip()
    {
        Vector2 currentEulerAngles = transform.eulerAngles;
        currentEulerAngles.y += 180;
        transform.eulerAngles = currentEulerAngles;

        if (isGrounded) myAnimator.SetTrigger("Turn");

        facingRight = !facingRight;
    }

    private void Jump()
    {
        myRigidbody2D.AddForce(new Vector2(0, jumpForce));

        GameObject jumpEffect = poolManager.GetCachedPrefab(jumpEffectID);
        jumpEffect.transform.position = feet.position;
        jumpEffect.SetActive(true);
    }

    private void Dash(float force)
    {
        isDashing = true;

        myRigidbody2D.velocity = Vector2.zero;

        Physics2D.gravity = new Vector2(gravityValue * transform.right.x, 0);
        myRigidbody2D.AddForce(new Vector2(force * transform.right.x, 0));
    }
    #endregion

    #region Attack
    private void Attack()
    {
        Dash(attackForce);
        attackTrigger.enabled = true;
    }

    public void EndAttack()
    {
        attackTrigger.enabled = false;
        isAttacking = false;
    }
    #endregion

    #region Future Self
    private void TransitToFuture()
    {
        Dash(dashForce);

        playerDummy = poolManager.GetCachedPrefab(playerDummyID);
        playerDummy.transform.SetPositionAndRotation(transform.position, transform.rotation);
        dummyIsFacingRight = facingRight;
        playerDummy.SetActive(true);

        futureSelfTimer = Time.time + futureSelfDuration;
        isTransiting = true;

        initialColor = mySpriteRenderer.color;
        mySpriteRenderer.color = futureSelfColor;
    }

    private void TransitToPresent()
    {
        isTransiting = false;

        transform.SetPositionAndRotation(playerDummy.transform.position, playerDummy.transform.rotation);
        facingRight = dummyIsFacingRight;
        playerDummy.SetActive(false);

        mySpriteRenderer.color = initialColor;
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

    #region Debug
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(feet.position, groundCheckRadius);
    }
#endif 
    #endregion
}
