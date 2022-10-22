using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class RTSController : MonoBehaviour
{
    [SerializeField] private Transform selectionAreaTransform;
    [SerializeField] private Camera cam;
    [SerializeField] private float zoomSensitivity;

    private Vector3 startPosition;
    private List<Survivor> selectedSurvivors;
    private List<HidingSpot> selectedHidingSpots;
    private Vector3 movePosition;
    private float minCameraZoom;
    private float maxCameraZoom;

    private void Awake() 
    {
        selectedSurvivors = new List<Survivor>();
        selectedHidingSpots = new List<HidingSpot>();
        selectionAreaTransform.gameObject.SetActive(false);
        minCameraZoom = 100f;
        maxCameraZoom = 5f;
        // movePosition = new Vector3(0, 0, 0);
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
            foreach (HidingSpot hidingSpot in selectedHidingSpots)
            {
                hidingSpot.SetSelected(false);
            }

            selectedSurvivors.Clear();
            selectedHidingSpots.Clear();

            // Select all units within selection area
            foreach (Collider2D collider in colliderArray)
            {
                Survivor survivor = collider.GetComponent<Survivor>();
                HidingSpot hidingSpot = collider.GetComponent<HidingSpot>();

                if (survivor != null)
                {
                    survivor.SetSelectedVisible(true);
                    selectedSurvivors.Add(survivor);
                }
                else if (hidingSpot != null)
                {
                    hidingSpot.SetSelected(true);
                    selectedHidingSpots.Add(hidingSpot);
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
            List<Vector3> targetPostionList = GetPositionListAround(movePosition, new float[] {2.5f, 5f, 7.5f}, new int[] {5, 10, 20});

            int targetPostionListIndex = 0;

            foreach (Survivor survivor in selectedSurvivors)
            {
                survivor.MoveToPosition(targetPostionList[targetPostionListIndex]);
                targetPostionListIndex = (targetPostionListIndex + 1) % targetPostionList.Count;
            }
        }

        float zoomChange = Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        if ((cam.orthographicSize <= minCameraZoom && zoomChange < 0) || (cam.orthographicSize > maxCameraZoom && zoomChange > 0))
        {
            cam.orthographicSize -= (Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity);
        }
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float[] ringDistanceArray, int[] ringPositionCountArray)
    {
        List<Vector3> positionList = new List<Vector3>();
        positionList.Add(startPosition);
        for (int i = 0; i < ringDistanceArray.Length; i++)
        {
            positionList.AddRange(GetPositionListAround(startPosition, ringDistanceArray[i], ringPositionCountArray[i]));
        }
        return positionList;
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float distance, int positionCount)
    {
        List<Vector3> positionList = new List<Vector3>();
        for (int i = 0; i < positionCount; i++)
        {
            float angle = i * (360f / positionCount);
            Vector3 dir = ApplyRotationToVector(new Vector3(1, 0), angle);
            Vector3 position = startPosition + dir * distance;
            positionList.Add(position);
        }
        return positionList;
    }

    private Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vec;
    }
}
