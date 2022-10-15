using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class RTSController : MonoBehaviour
{
    [SerializeField] private Transform selectionAreaTransform;

    private Vector3 startPosition;
    private List<Survivor> selectedSurvivors;
    private Vector3 movePosition;

    private void Awake() 
    {
        selectedSurvivors = new List<Survivor>();
        selectionAreaTransform.gameObject.SetActive(false);
        movePosition = new Vector3(0, 0, 0);
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
            startPosition = UtilsClass.GetMouseWorldPosition();
            selectionAreaTransform.gameObject.SetActive(true);
        }

        if (Input.GetMouseButton(0))        // If left mouse button is held down
        {
            Vector3 currentMousePosition = UtilsClass.GetMouseWorldPosition();
            Vector3 lowerLeft = new Vector3(
                Mathf.Min(startPosition.x, currentMousePosition.x),
                Mathf.Min(startPosition.y, currentMousePosition.y)
            );
            Vector3 upperRight = new Vector3(
                Mathf.Max(startPosition.x, currentMousePosition.x),
                Mathf.Max(startPosition.y, currentMousePosition.y)
            );

            selectionAreaTransform.position = lowerLeft;
            selectionAreaTransform.localScale = upperRight - lowerLeft;
        }

        if (Input.GetMouseButtonUp(0))      // If left mouse button is released
        {   
            selectionAreaTransform.gameObject.SetActive(false);

            Collider2D[] colliderArray = Physics2D.OverlapAreaAll(startPosition, UtilsClass.GetMouseWorldPosition());

            // Deselect all units before clearing the list.
            foreach (Survivor survivor in selectedSurvivors)
            {
                survivor.SetSelectedVisible(false);
            }

            selectedSurvivors.Clear();

            // Select all units within selection area
            foreach (Collider2D collider in colliderArray)
            {
                Survivor survivor = collider.GetComponent<Survivor>();
                if (survivor != null)
                {
                    survivor.SetSelectedVisible(true);
                    selectedSurvivors.Add(survivor);
                }
            }

            // Debug.Log($"Survivors selected: {selectedSurvivors.Count}");
        }

        if (Input.GetMouseButtonDown(1))
        {
            movePosition = UtilsClass.GetMouseWorldPosition();
        }

        if (Input.GetMouseButtonUp(1))
        {
            foreach (Survivor survivor in selectedSurvivors)
            {
                survivor.GetComponent<SurvivorMovementPathfinding>().SetMovePosition(movePosition);
            }
        }
    }
}
