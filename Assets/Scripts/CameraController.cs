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

    private void UpdatePlayerInputs()
    {
        float horozontalVelocity = Input.GetAxis("Horizontal") * moveSpeed;
        float verticalVelocity = Input.GetAxis("Vertical") * moveSpeed;

        body.velocity = new Vector3(horozontalVelocity, verticalVelocity);
    }
}
