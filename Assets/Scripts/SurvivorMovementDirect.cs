using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorMovementDirect : MonoBehaviour
{
    private Vector3 movePosition;
    private Rigidbody2D body;

    private void Awake() 
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDir = (movePosition - transform.position).normalized;
        body.velocity = moveDir;
    }

    public void SetMovePosition(Vector3 movePosition)
    {
        this.movePosition = movePosition;
    }
}
