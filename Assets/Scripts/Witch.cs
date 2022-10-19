using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Witch : MonoBehaviour
{
    protected enum PatrolState 
    {
        Patrolling, 
        Idle, 
        Chasing
    }

    private PatrolState patrolState;
    private Vector3 currentWaypoint;
    private LineOfSight lineOfSight;
    private Vector3 lastKnownPosition;
    private IMovePosition movePosition;
    private Rigidbody2D body;
    private bool isTouchingWall;

    [SerializeField] private float patrolRange;

    private void Awake() 
    {
        lineOfSight = transform.Find("LineOfSight").GetComponent<LineOfSight>();
        movePosition = GetComponent<IMovePosition>();
        body = GetComponent<Rigidbody2D>();
        patrolState = PatrolState.Patrolling;
        currentWaypoint = transform.position;
        lastKnownPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        List<GameObject> objects = lineOfSight.GetObjectsInSight();

        LookForPlayers(objects);
        UpdateDirection();
    }

    public void AlertToPlayerPosition(Vector3 playerPos)
    {
        lastKnownPosition = playerPos;
        patrolState = PatrolState.Chasing;
    }

    private void LookForPlayers(List<GameObject> objects)
    {
        if (objects.Count == 0)
        {
            // Debug.Log("No objects in sight!");
            if (patrolState == PatrolState.Chasing)
            {
                // Move to last known position until either survivors are found or no survivors can be seen.
                movePosition.SetMovePosition(lastKnownPosition);

                if (IsNearPosition(lastKnownPosition))
                {
                    patrolState = PatrolState.Patrolling;
                }
            }
            else if (patrolState == PatrolState.Patrolling)
            {
                PatrolArea();
            }

        }
        else
        {
            patrolState = PatrolState.Chasing;
            lastKnownPosition = objects[objects.Count - 1].transform.position;
            movePosition.SetMovePosition(lastKnownPosition);
        }

    }

    private void UpdateDirection()
    {
        Vector3 moveDirection = body.velocity.normalized;

        float horizontalMovement = moveDirection.x;
        float verticalMovement = moveDirection.y;

        if (math.abs(horizontalMovement) > math.abs(verticalMovement))
        {
            lineOfSight.SetDirection(
                (horizontalMovement < 0) 
                ? LineOfSight.Direction.Left
                : LineOfSight.Direction.Right
            );
        }
        else
        {
            lineOfSight.SetDirection(
                (verticalMovement < 0) 
                ? LineOfSight.Direction.Down
                : LineOfSight.Direction.Up
            );
        }
    }

    private void PatrolArea()
    {
        Vector3 patrolPoint = 
            (lastKnownPosition == null)
            ? transform.position
            : lastKnownPosition;

        if (IsNearPosition(currentWaypoint) || isTouchingWall)
        {
            isTouchingWall = false;
            SetRandomDirection();
            currentWaypoint = (lineOfSight.GetDirectionAsVector3() * patrolRange) + transform.position;
        }

        // Debug.Log(currentWaypoint);
        movePosition.SetMovePosition(currentWaypoint);
    }

    private void SetRandomDirection()
    {
        int selectedNumber = (int) math.round(UnityEngine.Random.Range(0, 4));

        switch (selectedNumber)
        {
            case 0:
                lineOfSight.SetDirection(LineOfSight.Direction.Left);
                break;
            case 1: 
                lineOfSight.SetDirection(LineOfSight.Direction.Right);
                break;
            case 2: 
                lineOfSight.SetDirection(LineOfSight.Direction.Up);
                break;
            default:
                lineOfSight.SetDirection(LineOfSight.Direction.Down);
                break;
        }
    }

    private bool IsNearPosition(Vector3 position)
    {
        // Debug.Log(Vector3.Distance(transform.position, position));
        return Vector3.Distance(transform.position, position) < 2.5f;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Wall")
            isTouchingWall = true;
        else if (other.gameObject.tag == "Player")
            other.gameObject.GetComponent<Survivor>().KillSurvivor();
    }
}

