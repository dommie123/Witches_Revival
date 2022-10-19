using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private Portal[] portals;

    [SerializeField] private int sourceIndex;
    [SerializeField] private int targetIndex;
    
    public bool PlayerIsExiting {get; set;}

    private void Awake() 
    {
        portals = Object.FindObjectsOfType<Portal>();
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
        
        exitPortal.PlayerIsExiting = true;
        other.transform.position = exitPortal.transform.position;   // Teleport entity to portal's position
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
