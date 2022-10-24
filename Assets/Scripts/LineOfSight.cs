using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public enum Direction {
        Up,
        Down,
        Left,
        Right
    }

    private Direction direction;
    private Collider2D parentCollider;
    private List<GameObject> objectsSighted;
    
    [SerializeField] private string targetTag;
    [SerializeField] private Transform parent;
    [SerializeField] private float sightRange;

    // Start is called before the first frame update
    void Awake()
    {
        parentCollider = parent.GetComponent<Collider2D>();
        objectsSighted = new List<GameObject>();
        direction = Direction.Right;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dirVector = GetDirectionAsVector3();
        RaycastHit2D[] results = new RaycastHit2D[10];

        int hitCount = parentCollider.Cast(dirVector, results, sightRange);
        Debug.DrawRay(parentCollider.transform.position, dirVector * sightRange, Color.blue, 0f, false);
        // Debug.Log(hitCount);

        objectsSighted.Clear();
        if (hitCount == 0)
        {
            return;
        }

        foreach (RaycastHit2D hit in results)
        {
            // Debug.Log(hit.rigidbody);
            if (!hit.collider)
            {
                continue;
            }

            if (hit.collider.gameObject.tag == targetTag)
            {
                objectsSighted.Add(hit.collider.gameObject);
            }
        }

    }

    public List<GameObject> GetObjectsInSight()
    {
        return objectsSighted;
    }

    public void SetDirection(Direction dir)
    {
        direction = dir;
    }

    public Direction GetDirection()
    {
        return direction;
    }

    public Vector3 GetDirectionAsVector3()
    {
        switch (direction)
        {
            case Direction.Up: 
                return Vector3.up;
            case Direction.Down:
                return Vector3.down;
            case Direction.Left:
                return Vector3.left;
            case Direction.Right:
                return Vector3.right;
            default:
                return Vector3.zero;
        }
    }
}
