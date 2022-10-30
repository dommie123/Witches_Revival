using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager instance;
    
    private float cameraSpeed;
    private float zoomSensitivity;
    private float gameVolume;
    private float screenBrightness;
    private bool isFullScreen;
    private int2 screenResolution;

    private void Awake() 
    {
        cameraSpeed = 10f;
        zoomSensitivity = 7.5f;
        screenBrightness = 1f;
        gameVolume = 1f;
        isFullScreen = true;
        screenResolution = new int2(1366, 768);

        SetGameResolution(screenResolution);

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

    public float GetCameraSpeed()
    {
        return cameraSpeed;
    }

    public void SetCameraSpeed(float cameraSpeed)
    {
        this.cameraSpeed = cameraSpeed;
    }

    public float GetZoomSensitivity()
    {
        return zoomSensitivity;
    }

    public void SetZoomSensitivity(float zoomSensitivity)
    {
        this.zoomSensitivity = zoomSensitivity;
    }
    public void SetGameVolume(float volume)
    {
        gameVolume = volume;
    }

    public void SetScreenBrightness(float brightness)
    {
        screenBrightness = brightness;
    }

    public void SetGameResolution(TMP_Dropdown change)
    {
        int width = int.Parse(change.captionText.text.Split("x")[0].Trim());
        int height = int.Parse(change.captionText.text.Split("x")[1].Trim());

        int2 newResolution = new int2(width, height);
        SetGameResolution(newResolution);
    }

    // Test the following functions in the build. They do not work in the editor.
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
