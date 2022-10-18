using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class UnitMovementPathfinding : MonoBehaviour, IMovePosition
{
    [SerializeField] private float moveSpeed;

    private List<Vector3> pathVectorList;
    private int pathIndex = -1;
    private Rigidbody2D body;

    private void Awake() 
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (pathIndex != -1)
        {
            Vector3 nextPathPosition = pathVectorList[pathIndex];
            Vector3 moveVelocity = (nextPathPosition - transform.position).normalized;
            body.velocity = moveVelocity * moveSpeed;

            float reachedPathPositionDistance = 1f;
            if (Vector3.Distance(transform.position, nextPathPosition) < reachedPathPositionDistance)
            {
                pathIndex++;
                if (pathIndex >= pathVectorList.Count)
                {
                    // End of path
                    pathIndex = -1;
                }
            }
        }
        else
        {
            // Idle
            body.velocity = Vector3.zero;
        }
    }

    public void SetMovePosition(Vector3 movePosition)
    {
        //this.movePosition = movePosition;
        pathVectorList = GridPathfinding.instance.GetPathRouteAsVectorList(transform.position, movePosition);
        if (pathVectorList.Count > 0)
        {
            pathIndex = 0;
        }
    }
}
