using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : InteractableScript
{
    [Header("Lever Properties")]
    [Space]
    [SerializeField]
    private bool activateOnce = true;

    [Space]
    [SerializeField]
    private GameObject[] objectsToActivate;

    [Space]
    [SerializeField]
    private Sprite activeSprite;

    [Header("Shake Settings")]
    [Space]
    [SerializeField]
    private bool doShake;
    [SerializeField]
    private float shakeForce = .2f;
    [SerializeField]
    private float shakeDuration = .7f;

    [Header("Audio")]
    [Space]
    [SerializeField]
    private string leverSound = "Lever";

    private bool active;

    private Sprite initialSprite;

    private CameraScript cameraScript;

    protected override void Awake()
    {
        base.Awake();

        cameraScript = Camera.main.GetComponent<CameraScript>();
    }

    private void Start()
    {
        initialSprite = mySpriteRenderer.sprite;
    }

    public override void Interact()
    {
        mySpriteRenderer.sprite = active ? initialSprite : activeSprite;

        audioManager.PlaySound(leverSound, gameObject.name);

        if (objectsToActivate.Length > 0)
        {
            foreach (GameObject objectToActivate in objectsToActivate)
            {
                if (objectToActivate.activeInHierarchy)
                {
                    if (objectToActivate.GetComponent<IActivable>() != null) objectToActivate.GetComponent<IActivable>().Activate();
                    else Debug.LogError(objectToActivate.name + "does not contain a IActivable interface."); 
                }
            }
        }
        else Debug.LogError("Didn't set any object to activate in " + gameObject.name + ".");

        if (activateOnce) myCollider2D.enabled = false;
        else active = !active;

        if (doShake) cameraScript.ShakeCamera(shakeForce, shakeDuration);
    }
}
