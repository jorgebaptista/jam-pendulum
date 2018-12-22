using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField]
    private Animator screenFadeAnimator;

    [Space]
    [SerializeField]
    private bool fadeOnStart = true;

    public static SceneManagerScript instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            if (fadeOnStart && !screenFadeAnimator.gameObject.activeInHierarchy) screenFadeAnimator.gameObject.SetActive(true);
        }
        else Destroy(gameObject);
    }

    public void Fade()
    {
        screenFadeAnimator.SetTrigger("Reset");
    }

    #region Scene Management
    public void RestartScene(bool fade)
    {
        if (fade) StartCoroutine(LoadSceneFade(SceneManager.GetActiveScene().name));
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(LoadSceneFade(scene));
    }

    private IEnumerator LoadSceneFade(string scene)
    {
        if (!screenFadeAnimator.gameObject.activeInHierarchy) screenFadeAnimator.gameObject.SetActive(true);
        else screenFadeAnimator.SetTrigger("Fade");

        AsyncOperation sceneToLoad = SceneManager.LoadSceneAsync(scene);
        sceneToLoad.allowSceneActivation = false;

        Time.timeScale = 0;

        while (sceneToLoad.progress < .9f)
        {
            yield return null;
        }

        Time.timeScale = 1;

        sceneToLoad.allowSceneActivation = true;

        screenFadeAnimator.SetTrigger("Fade");
    }

    public void Quit()
    {
        Application.Quit();
    }
    #endregion
}
