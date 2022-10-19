using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private List<Survivor> survivors;

    public List<Survivor> escapedSurvivors;

    private void Awake()
    {
        instance = this;
        survivors = new List<Survivor>();
        escapedSurvivors = new List<Survivor>();

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
        Debug.Log("I won!");
    }  

    public void LoseGame()
    {
        Debug.Log("Game Over!");
    }

    public void QueueEscapedSurvivor(Survivor survivor)
    {
        escapedSurvivors.Add(survivor);
    }
}
