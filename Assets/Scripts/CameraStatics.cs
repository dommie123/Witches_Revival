using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStatics : MonoBehaviour
{
    private static CameraController cam;

    private void Awake() 
    {
        cam = GetComponent<CameraController>();
    }

    public static void SnapCameraToPosition(Vector3 position)
    {
        if (cam != null)
        {
            cam.SnapToPosition(position);
        }
    }
}
