using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string levelOne;
    public GameObject optionsPanel;
    public GameObject instructionsPanel;
    public GameObject mainMenuPanel;

    public void Play()
    {
        SceneManager.LoadScene(levelOne);
    }

    public void HowToPlay()
    {
        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    public void Options()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void GoBack()
    {
        optionsPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}