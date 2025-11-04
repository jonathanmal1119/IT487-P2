using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string startingSceneName;


    public void StartGame()
    {
        SceneManager.LoadScene(startingSceneName);
    }


    public void QuitGame()
    {
        Application.Quit();
    }

}
