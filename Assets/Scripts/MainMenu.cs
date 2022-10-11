using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string levelOne;

    public string howToPlay;

    public string options;

    public void Play()
    {
        SceneManager.LoadScene(levelOne);
    }

    public void HowToPlay()
    {
        SceneManager.LoadScene(howToPlay);
    }

    public void Options()
    {
        SceneManager.LoadScene(options);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}