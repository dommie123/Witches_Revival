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

    private List<Survivor> survivors;
    private bool losingSequenceActive;
    private bool winningSequenceActive;
    private AudioSource jumpscareSound;
    private int numSurvivorsLeft;
    private int numSurvivorsEscaped;

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
        // Debug.Log("I won!");
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
        // Debug.Log("Game Over!");
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
}
