using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

public class GridPathfinding : MonoBehaviour
{
    public static GridPathfinding instance;
    
    [SerializeField] private Pathfinding pathfinder;
    [SerializeField] private int2 gridSize;

    private void Awake() 
    {
        instance = this;
    }

    public List<Vector3> GetPathRouteAsVectorList(Vector3 startPosition, Vector3 endPosition)
    {
        List<Vector3> convertedPath = new List<Vector3>();
        NativeList<int2> path = FindPath(startPosition, endPosition);

        foreach (int2 point in path)
        {
            convertedPath.Add(new Vector3(point.x, point.y));
        }

        path.Dispose();

        return convertedPath;
    }

    public int2 GetGridSize()
    {
        return gridSize;
    }

    private NativeList<int2> FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        int2 startCoords = new int2((int) math.round(startPosition.x), (int) math.round(startPosition.y));
        int2 endCoords = new int2((int) math.round(endPosition.x), (int) math.round(endPosition.y));

        return pathfinder.FindPath(startCoords, endCoords, gridSize);
    }
}
