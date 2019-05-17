using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
	public string DefaultSceneName;

	public void LoadScene()
	{
		if (!string.IsNullOrEmpty(DefaultSceneName))
			SceneManager.LoadScene(DefaultSceneName, LoadSceneMode.Single);
	}

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}

	public void LoadScene(int sceneIndex)
	{
		SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
	}

	public void LoadNextScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void Quit()
	{
		Application.Quit();
	}
}
