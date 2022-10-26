using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject instructionsPanel;
    public GameObject pauseMenuPanel;
    // public GameObject hud;

    public void HowToPlay()
    {
        pauseMenuPanel.SetActive(false);
        // hud.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    public void Options()
    {
        // hud.SetActive(false);
        pauseMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void GoBack()
    {
        optionsPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
        // hud.SetActive(true);
    }
}
