using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorMovementDirect : MonoBehaviour, IMovePosition
{
    [SerializeField] private float moveSpeed;

    private Vector3 movePosition;
    private Rigidbody2D body;

    private void Awake() 
    {
        body = GetComponent<Rigidbody2D>();
        movePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDir = Vector3.Normalize(movePosition - transform.position);

        if (Vector3.Distance(movePosition, transform.position) < 1f)
            moveDir = Vector3.zero;     // Stop moving when close to move position

        body.velocity = moveDir * moveSpeed;
        Debug.Log(moveDir);
    }

    public void SetMovePosition(Vector3 movePosition)
    {
        this.movePosition = movePosition;
    }
}
