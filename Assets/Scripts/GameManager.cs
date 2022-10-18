using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;

        // TODO set up UI elements and behaviors
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void WinGame()
    {
        Debug.Log("I won!");
    }  

    public void LoseGame()
    {
        Debug.Log("Game Over!");
    }
}
