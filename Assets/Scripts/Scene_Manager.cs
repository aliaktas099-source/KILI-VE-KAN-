using UnityEngine;

public class Scene_Manager : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (!PlayerPrefs.HasKey("Level")) PlayerPrefs.SetInt("Level", 1);
    }
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    }
    public void LoadScene(int Level)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game");
        PlayerPrefs.SetInt("Level", Level);
    }
}
