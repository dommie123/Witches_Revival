using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class RTSController : MonoBehaviour
{
    [SerializeField] private GameUtils utils;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerInputs();
    }

    private void UpdatePlayerInputs()
    {
        if (Input.GetMouseButtonDown(0))    // If left mouse button is pressed
        {
            startPosition = utils.GetMouseWorldPosition3D();
        }

        if (Input.GetMouseButtonUp(0))      // If left mouse button is released
        {   
            Debug.Log(utils.GetMouseWorldPosition3D() + " " + startPosition);
            Collider[] colliderArray = Physics.OverlapBox(startPosition, utils.GetMouseWorldPosition3D());
            Debug.Log("==========");
            foreach (Collider collider in colliderArray)
            {
                Debug.Log(collider);
            }
        }
    }
}
