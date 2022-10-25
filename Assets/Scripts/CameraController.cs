using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerInputs();
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
}
