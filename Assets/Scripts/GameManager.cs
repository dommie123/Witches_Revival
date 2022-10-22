using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private float jumpscareCooldown;
    [SerializeField] private string mainLevel;
    private List<Survivor> survivors;
    private bool losingSequenceActive;
    private bool winningSequenceActive;

    public List<Survivor> escapedSurvivors;
    public GameObject jumpscarePanel;
    public GameObject gameOverPanel;
    public GameObject hud;
    public GameObject winPanel;
    
    private void Awake()
    {
        instance = this;
        survivors = new List<Survivor>();
        escapedSurvivors = new List<Survivor>();
        losingSequenceActive = false;
        winningSequenceActive = false;

        Survivor[] survivorsArr = Object.FindObjectsOfType<Survivor>();
        foreach (Survivor survivor in survivorsArr)
        {
            survivors.Add(survivor);
        }

        // TODO set up UI elements and behaviors
    }

    // Update is called once per frame
    private void Update()
    {
        // If all of the survivors are dead, the game is over.
        bool gameLost = true;
        foreach (Survivor survivor in survivors)
        {
            if (!survivor.GetIsDead())
            {
                gameLost = false;
                break;
            }
        }


        // If all of the survivors escape the maze, the player wins the game.
        int activeSurvivors = survivors.Count;
        foreach (Survivor survivor in survivors)
        {
            if (survivor.GetIsDead())
            {
                activeSurvivors--;
            }
        }

        if (activeSurvivors == escapedSurvivors.Count && !gameLost)
        {
            WinGame();
        }
        else if (gameLost)
        {
            LoseGame();
        }
    }

    public void WinGame()
    {
        // Debug.Log("I won!");
        if (!winningSequenceActive)
        {
            hud.SetActive(false);
            winPanel.SetActive(true);
            winningSequenceActive = true;
            return;
        }
    }  

    public void LoseGame()
    {
        // Debug.Log("Game Over!");
        // Disable hud, enable jumpscare
        if (!losingSequenceActive)
        {
            hud.SetActive(false);
            jumpscarePanel.SetActive(true);
            losingSequenceActive = true;
            // TODO play scream sound here
            return;
        }

        // This check prevents the jumpscare panel from looping over and over.
        if (gameOverPanel.activeSelf)
        {
            return;
        }

        // After a few seconds, disable jumpscare and enable game over panel
        if (jumpscareCooldown <= 0f)
        {
            jumpscarePanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }
        else
        {
            jumpscareCooldown -= Time.deltaTime;
        }

    }

    public void QueueEscapedSurvivor(Survivor survivor)
    {
        escapedSurvivors.Add(survivor);
    }

    public void ExitToTitleScreen()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(mainLevel);
    }
}
