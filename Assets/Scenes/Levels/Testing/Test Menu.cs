using UnityEngine;

public class TestMenu : MonoBehaviour
{
    public void OpenGame(string sceneName) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
