using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChestScript : InteractableScript
{
    [SerializeField]
    private Sprite openChestSprite;

    [Space]
    [SerializeField]
    private string openChestSound = "Chest";

    private Animator myAnimator;

    protected override void Awake()
    {
        base.Awake();

        myAnimator = GetComponent<Animator>();
    }

    public override void Interact()
    {
        myAnimator.enabled = true;

        audioManager.PlaySound(openChestSound, gameObject.name);

        myCollider2D.enabled = false;
    }

    public void TurnCollectible()
    {
        canCollect = true;

        myCollider2D.enabled = true;
    }

    public virtual void Collect(PlayerScript playerScript)
    {
        myAnimator.enabled = false;
        mySpriteRenderer.sprite = openChestSprite;
        myCollider2D.enabled = false;
    }
}
