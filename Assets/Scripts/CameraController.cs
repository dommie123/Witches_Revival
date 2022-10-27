using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float moveSpeed;
    private Rigidbody2D body;
    private OptionsManager options;

    // Start is called before the first frame update
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        // options = GameObject.Find("GameOptions").GetComponent<OptionsManager>();
        
        // if (options != null)
        // {
        //     moveSpeed = options.GetCameraSpeed();
        // }
        // else
        // {
        //     moveSpeed = 10;
        // }
    }

    private void OnEnable() 
    {
        RefreshOptions();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.GameIsPaused())
            return;

        UpdatePlayerInputs();

        if (options == null)
        {
            return;
        }

        if (moveSpeed != options.GetCameraSpeed())
        {
            RefreshOptions();
        }
    }

    public void SnapToPosition(Vector3 position)
    {
        Vector3 newCameraPos = new Vector3(
            position.x, 
            position.y, 
            transform.position.z     // Keep original camera z coordinate
        );

        this.transform.position = newCameraPos;
    }

    private void UpdatePlayerInputs()
    {
        float horozontalVelocity = Input.GetAxis("Horizontal") * moveSpeed;
        float verticalVelocity = Input.GetAxis("Vertical") * moveSpeed;

        body.velocity = new Vector3(horozontalVelocity, verticalVelocity);
    }

    private void RefreshOptions()
    {
        GameObject gameOptions = GameObject.Find("GameOptions");//.GetComponent<OptionsManager>();
        
        if (gameOptions != null)
        {
            options = gameOptions.GetComponent<OptionsManager>();
            moveSpeed = options.GetCameraSpeed();
        }
        else
        {
            moveSpeed = 10;
        }
    }
}
