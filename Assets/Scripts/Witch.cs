using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Witch : MonoBehaviour
{
    public event EventHandler OnKillPlayer;

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
    private float wallCooldown;
    private float teleportCooldown;
    private float chaseCooldown;
    private Transform[] tpLocations;
    private bool playerWasSpotted;

    [SerializeField] private float patrolRange;
    // [SerializeField] private float minDistanceToPlayer;
    [SerializeField] private GameObject tpLocationList;
    [SerializeField] private float teleportTimer;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip chasingBgm;
    [SerializeField] private AudioClip defaultBgm;

    private void Awake() 
    {
        lineOfSight = transform.Find("LineOfSight").GetComponent<LineOfSight>();
        movePosition = GetComponent<IMovePosition>();
        body = GetComponent<Rigidbody2D>();
        patrolState = PatrolState.Patrolling;
        currentWaypoint = transform.position;
        lastKnownPosition = transform.position;
        wallCooldown = 0f;
        chaseCooldown = 0f;
        teleportCooldown = teleportTimer;
        playerWasSpotted = false;

        if (tpLocationList)
        {
            tpLocations = tpLocationList.GetComponentsInChildren<Transform>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        List<GameObject> objects = lineOfSight.GetObjectsInSight();

        LookForPlayers(objects);
        UpdateDirection();
        UpdateTeleportStatus();
    }

    public void AlertToPlayerPosition(Vector3 playerPos)
    {
        if (!playerWasSpotted)
        {
            playerWasSpotted = true;
            patrolState = PatrolState.Chasing;
            ChangeSongs();
            chaseCooldown = 10f;
        }
        lastKnownPosition = playerPos;
    }

    private void LookForPlayers(List<GameObject> objects)
    {
        if (chaseCooldown > 0f)
        {
            chaseCooldown -= Time.deltaTime;
        }

        if (objects.Count == 0)
        {
            // Debug.Log("No objects in sight!");
            if (patrolState == PatrolState.Chasing)
            {
                // Move to last known position until either survivors are found or no survivors can be seen.
                movePosition.SetMovePosition(lastKnownPosition);

                if (IsNearPosition(lastKnownPosition) || chaseCooldown <= 0f)
                {
                    patrolState = PatrolState.Patrolling;
                    playerWasSpotted = false;
                    ChangeSongs();
                }
            }
            else if (patrolState == PatrolState.Patrolling)
            {
                PatrolArea();
            }

        }
        else if (!playerWasSpotted)
        {
            playerWasSpotted = true;
            ChangeSongs();
            patrolState = PatrolState.Chasing;

        }
        else
        {
            chaseCooldown = 10f;
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
            // isTouchingWall = false;
            if (wallCooldown > 0f)
            {
                wallCooldown -= Time.deltaTime;
            }
            else if (isTouchingWall)
            {
                SetInvertedDirection(lineOfSight.GetDirection());
                currentWaypoint = (lineOfSight.GetDirectionAsVector3() * patrolRange) + transform.position;
            }
            else
            {
                SetRandomDirection();
                currentWaypoint = (lineOfSight.GetDirectionAsVector3() * patrolRange) + transform.position;
            }
            
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

    private void SetInvertedDirection(LineOfSight.Direction currentDirection)
    {
        if (currentDirection == LineOfSight.Direction.Left)
        {
            lineOfSight.SetDirection(LineOfSight.Direction.Right);
        }
        else if (currentDirection == LineOfSight.Direction.Right)
        {
            lineOfSight.SetDirection(LineOfSight.Direction.Left);
        }
        else if (currentDirection == LineOfSight.Direction.Up)
        {
            lineOfSight.SetDirection(LineOfSight.Direction.Down);
        }
        else
        {
            lineOfSight.SetDirection(LineOfSight.Direction.Up);
        }
    }

    private bool IsNearPosition(Vector3 position)
    {
        // Debug.Log(Vector3.Distance(transform.position, position));
        return Vector3.Distance(transform.position, position) < 2.5f;
    }

    private void UpdateTeleportStatus()
    {
        if (patrolState != PatrolState.Chasing)
        {
            if (teleportCooldown <= 0)
            {
                TeleportToRandomPosition();
                teleportCooldown = teleportTimer;
            }
            else
            {
                teleportCooldown -= Time.deltaTime;
            }
        }
    }

    private void TeleportToRandomPosition()
    {
        int randIndex = UnityEngine.Random.Range(0, tpLocations.Length);
        this.transform.position = tpLocations[randIndex].position;
        SetRandomDirection();
        currentWaypoint = (lineOfSight.GetDirectionAsVector3() * patrolRange) + transform.position;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Wall")
        {
            SetInvertedDirection(lineOfSight.GetDirection());
            isTouchingWall = true;
            wallCooldown = 2f;
        }
        else if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Survivor>().KillSurvivor();
            OnKillPlayer?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Wall")
        {
            isTouchingWall = false;
        }
    }

    private void ChangeSongs()
    {
        bgmSource.Stop();

        if (patrolState == PatrolState.Chasing)
        {
            bgmSource.clip = chasingBgm;
        }
        else
        {
            bgmSource.clip = defaultBgm;
        }

        bgmSource.Play();
    }
}

