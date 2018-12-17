using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour
{
    [Header("HUD")]
    [Space]
    [SerializeField]
    private Image lifeBar;
    [SerializeField]
    private float lifeBarSpeed = 3f;

    [Space]
    [SerializeField]
    private GameObject timerBarGroup;
    [SerializeField]
    private Image timerBar;
    [SerializeField]
    private float timerBarSpeed = 5f;

    //sssssssssssssssssss
    [SerializeField]
    private GameObject fadePrefab;

    private void Awake()
    {
        fadePrefab.SetActive(true);
    }

    public void UpdateLifeBar(float lifePercentage)
    {
        StopCoroutine("UpdateLifeBarImage");
        StartCoroutine("UpdateLifeBarImage", lifePercentage);
    }

    private IEnumerator UpdateLifeBarImage(float percentage)
    {
        while (percentage != lifeBar.fillAmount)
        {
            lifeBar.fillAmount = Mathf.MoveTowards(lifeBar.fillAmount, percentage, Time.deltaTime * lifeBarSpeed);

            yield return null;
        }
    }

    public void ToggleTimerBar(bool toggle)
    {
        if (toggle) timerBarGroup.SetActive(true);
        else timerBarGroup.SetActive(false);
    }

    public void UpdateTimerBar(float timerPercentage)
    {
        StopCoroutine("UpdateTimerBarImage");
        StartCoroutine("UpdateTimerBarImage", timerPercentage);
    }

    private IEnumerator UpdateTimerBarImage(float percentage)
    {
        while (percentage != timerBar.fillAmount)
        {
            timerBar.fillAmount = Mathf.MoveTowards(timerBar.fillAmount, percentage, Time.deltaTime * timerBarSpeed);

            yield return null;
        }
    }
}
