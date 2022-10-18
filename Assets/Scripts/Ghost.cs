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

    [SerializeField] private float patrolRange;
    [SerializeField] private Witch witch;

    private void Awake() 
    {
        lineOfSight = transform.Find("LineOfSight").GetComponent<LineOfSight>();
        movePosition = GetComponent<IMovePosition>();
        body = GetComponent<Rigidbody2D>();
        currentWaypoint = transform.position;
        alertGraphic = transform.Find("AlertGraphic").gameObject;
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

        if (players.Count > 0)
        {
            alertGraphic.SetActive(true);
            witch.AlertToPlayerPosition(players[players.Count - 1].transform.position);
        }
        else
        {
            alertGraphic.SetActive(false);
        }
    }

    private void PatrolArea()
    {        
        if (IsNearPosition(currentWaypoint) || isTouchingWall)
        {
            isTouchingWall = false;
            SetRandomDirection();
            currentWaypoint = (lineOfSight.GetDirectionAsVector3() * patrolRange) + transform.position;
        }

        Debug.Log(currentWaypoint);
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
        Debug.Log(Vector3.Distance(transform.position, position));
        return Vector3.Distance(transform.position, position) < 2.5f;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Wall")
            isTouchingWall = true;
    }
}
