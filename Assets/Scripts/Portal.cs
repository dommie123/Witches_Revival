using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private Portal[] portals;
    private AudioSource portalWoosh;

    [SerializeField] private int sourceIndex;
    [SerializeField] private int targetIndex;
    
    public bool PlayerIsExiting {get; set;}

    private void Awake() 
    {
        portals = Object.FindObjectsOfType<Portal>();
        portalWoosh = GetComponent<AudioSource>();
        PlayerIsExiting = false;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        // if (other.gameObject.layer != entityMask)
        //     return;
        
        if (PlayerIsExiting)
            return;
        
        Portal exitPortal = null;

        foreach (Portal portal in portals)
        {
            if (portal.GetSourceIndex() == targetIndex)
            {
                exitPortal = portal;
                break;
            }
        }

        if (exitPortal == null)
            return;
        
        portalWoosh.Play();
        exitPortal.PlayerIsExiting = true;
        other.transform.position = exitPortal.transform.position;   // Teleport entity to portal's position

        if (other.gameObject.tag == "Player")
        {
            CameraStatics.SnapCameraToPosition(exitPortal.transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        PlayerIsExiting = false;    // This helps to make the portals two-way
    }

    public int GetSourceIndex()
    {
        return sourceIndex;
    }
}
