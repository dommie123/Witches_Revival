using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private float jumpscareCooldown;
    [SerializeField] private string mainLevel;
    [SerializeField] private AudioSource bgm;
    [SerializeField] private GameObject ghost;
    [SerializeField] private GameObject witch;
    private OptionsManager options;

    private List<Survivor> survivors;
    private bool losingSequenceActive;
    private bool winningSequenceActive;
    private bool gamePaused;
    private AudioSource jumpscareSound;
    private int numSurvivorsLeft;
    private int numSurvivorsEscaped;

    public List<Survivor> escapedSurvivors;
    public GameObject jumpscarePanel;
    public GameObject gameOverPanel;
    public GameObject hud;
    public GameObject winPanel;
    public GameObject pauseMenu;
    public GameObject optionsPanel;
    public GameObject instructionsPanel;
    
    private void Awake()
    {
        // options = GameObject.Find("GameOptions").GetComponent<OptionsManager>();
        instance = this;
        survivors = new List<Survivor>();
        escapedSurvivors = new List<Survivor>();
        losingSequenceActive = false;
        winningSequenceActive = false;
        gamePaused = false;
        jumpscareSound = GetComponent<AudioSource>();

        Survivor[] survivorsArr = Object.FindObjectsOfType<Survivor>();
        foreach (Survivor survivor in survivorsArr)
        {
            survivors.Add(survivor);
        }

        numSurvivorsLeft = survivors.Count;
        numSurvivorsEscaped = escapedSurvivors.Count;

        if (SceneManager.GetActiveScene().name != "TestScene2")
        {
            SpawnEnemies();
        }

        HUDManager.instance.UpdateSurvivorText(numSurvivorsEscaped, numSurvivorsLeft);
    }

    private void OnEnable() 
    {
        options = GameObject.Find("GameOptions").GetComponent<OptionsManager>();
        
        if (options != null)
        {
            Debug.Log(
                $"Camera Speed: {options.GetCameraSpeed()} " + 
                $"Zoom Sensitivity: {options.GetZoomSensitivity()}"
            );
        }
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

        RefreshSurvivorCounts();

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
        if (!winningSequenceActive)
        {
            hud.SetActive(false);
            winPanel.SetActive(true);
            bgm.Stop();
            winningSequenceActive = true;
            return;
        }
    }  

    public void LoseGame()
    {
        // Disable hud, enable jumpscare
        if (!losingSequenceActive)
        {
            hud.SetActive(false);
            jumpscarePanel.SetActive(true);
            losingSequenceActive = true;
            bgm.Stop();
            jumpscareSound.Play();
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

    // We need this because the HUD manager doesn't want to wake up before the Game Manger for some reason.
    public void HUDManagerIsAwake()
    {
        HUDManager.instance.UpdateSurvivorText(numSurvivorsEscaped, numSurvivorsLeft);
    }

    public void PauseGame()
    {
        if (gamePaused && !OptionsOrInstructionsActive())
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
        else if (!gamePaused)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
        
        if (!OptionsOrInstructionsActive())
        {
            gamePaused = !gamePaused;
        }
    }

    public bool GameIsPaused()
    {
        return gamePaused;
    }

    private void RefreshSurvivorCounts()
    {
        int survivorsAlive = survivors.Count;

        foreach (Survivor survivor in survivors)
        {
            if (survivor.GetIsDead())
            {
                survivorsAlive--;
            }
        }

        if (numSurvivorsLeft != survivorsAlive || numSurvivorsEscaped != escapedSurvivors.Count)
        {
            numSurvivorsLeft = survivorsAlive;
            numSurvivorsEscaped = escapedSurvivors.Count;
            HUDManager.instance.UpdateSurvivorText(numSurvivorsEscaped, numSurvivorsLeft);
        }   
    }

    private void SpawnEnemies()
    {
        SpawnLocation[] spawnLocations = Object.FindObjectsOfType<SpawnLocation>();

        foreach (SpawnLocation location in spawnLocations)
        {
            SpawnEnemy(location.transform.position);
        }
    }

    private void SpawnEnemy(Vector3 spawnLocation)
    {
        Instantiate(ghost, spawnLocation, Quaternion.identity);
    }

    private bool OptionsOrInstructionsActive()
    {
        return optionsPanel.activeInHierarchy || instructionsPanel.activeInHierarchy;
    }
}
