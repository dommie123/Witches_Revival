using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GameUtils : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    public Vector3 GetMouseWorldPosition3D()
    {
        Vector3 currentPosition = UtilsClass.GetMouseWorldPosition();
        float currentDepth = 12f;
        RaycastHit[] hits = Physics.RaycastAll(cameraTransform.position, cameraTransform.forward, currentDepth);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.position.z < currentDepth)
            {
                currentDepth = hit.transform.position.z;
            }
        }

        currentPosition += new Vector3(0, 0, currentDepth);
        return currentPosition;
    }   
}
