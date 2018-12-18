using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableScript : MonoBehaviour
{
    [Header("General")]
    [Space]
    [SerializeField]
    protected bool canFSInteract;

    public bool CanFSInteract
    {
        get { return canFSInteract; }
    }

    protected bool canCollect;

    public bool CanCollect
    {
        get { return canCollect; }
    }

    protected SpriteRenderer mySpriteRenderer;
    protected Collider2D myCollider2D;

    protected AudioManagerScript audioManager;

    protected virtual void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myCollider2D = GetComponent<Collider2D>();

        audioManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<AudioManagerScript>();
    }

    public abstract void Interact();
}
