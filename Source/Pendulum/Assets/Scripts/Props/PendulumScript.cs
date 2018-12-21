using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumScript : MonoBehaviour, IActivable
{
    [Header("Properties")]
    [Space]
    [SerializeField]
    private string pendulumSound = "Pendulum";

    private Animator myAnimator;

    private AudioManagerScript audioManager;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();

        audioManager = GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<AudioManagerScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) audioManager.PlaySound(pendulumSound, gameObject.name);
    }

    public void Activate()
    {
        transform.GetChild(0).gameObject.SetActive(false);

        myAnimator.enabled = true;
    }
}
