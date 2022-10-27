using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Ghost : MonoBehaviour
{
    private Vector3 currentWaypoint;
    private LineOfSight lineOfSight;
    private IMovePosition movePosition;
    private Rigidbody2D body;
    private bool isTouchingWall;
    private GameObject alertGraphic;
    private AudioSource alertSFX;
    private Witch witch;
    private float wallCooldown;
    private bool playerWasSpotted;

    [SerializeField] private float patrolRange;

    private void Awake() 
    {
        lineOfSight = transform.Find("LineOfSight").GetComponent<LineOfSight>();
        movePosition = GetComponent<IMovePosition>();
        body = GetComponent<Rigidbody2D>();
        currentWaypoint = transform.position;
        alertGraphic = transform.Find("AlertGraphic").gameObject;
        alertSFX = GetComponent<AudioSource>();
        witch = Object.FindObjectOfType<Witch>();
        wallCooldown = 0f;
        playerWasSpotted = false;
    }

    // Update is called once per frame
    private void Update()
    {
        List<GameObject> players = lineOfSight.GetObjectsInSight();
        CheckForPlayers(players);
    }

    private void CheckForPlayers(List<GameObject> players)
    {
        PatrolArea();

        if (players.Count > 0 && !playerWasSpotted)
        {
            playerWasSpotted = true;
            alertGraphic.SetActive(true);
            alertSFX.Play();
            witch.AlertToPlayerPosition(players[players.Count - 1].transform.position);
        }
        else if (players.Count == 0)
        {
            playerWasSpotted = false;
            alertGraphic.SetActive(false);
        }
    }

    private void PatrolArea()
    {        
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

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Wall")
            isTouchingWall = true;
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.tag == "Wall")
            isTouchingWall = false;
    }
}
