using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelChanger : MonoBehaviour {
	public Animator animator;

	private int levelToLoad;

	private void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			FadeToLevel (1);
		}
	}
	public void FadeToLevel (int levelIndex)
	{
		levelToLoad = levelIndex;
		animator.SetTrigger ("FadeOut");
	}

	public void OnFadeComplete() {
		SceneManager.LoadScene (levelToLoad);
	}
}
