using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager instance;
    
    private float gameVolume;
    private float screenBrightness;
    private bool isFullScreen;
    private int2 screenResolution;

    private void Awake() 
    {
        screenBrightness = 1f;
        gameVolume = 1f;
        isFullScreen = true;

        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);    
    }

    private void Update() 
    {
        UpdateGameSettings();
    }

    private void UpdateGameSettings()
    {
        if (AudioListener.volume != gameVolume)
            AudioListener.volume = gameVolume;
        if (Screen.brightness != screenBrightness)
            Screen.brightness = screenBrightness;
        
    }

    public void SetGameVolume(float volume)
    {
        gameVolume = volume;
    }

    public void SetScreenBrightness(float brightness)
    {
        screenBrightness = brightness;
    }

    public void SetGameResolution(int2 resolution)
    {
        screenResolution = resolution;
        Screen.SetResolution(resolution.x, resolution.y, isFullScreen);
    }

    public void SetFullScreen(bool fullscreen)
    {
        isFullScreen = fullscreen;
        Screen.SetResolution(screenResolution.x, screenResolution.y, isFullScreen);
    }
}
