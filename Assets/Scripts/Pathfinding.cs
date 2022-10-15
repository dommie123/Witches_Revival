using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using CodeMonkey.Utils;

public class Pathfinding : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private void Start() 
    {
        //FindPath(new int2(0, 0), new int2(3, 1));
        FunctionPeriodic.Create(() => {
            float startTime = Time.realtimeSinceStartup;

            int findPathJobCount = 5;
            NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(findPathJobCount, Allocator.TempJob);

            for (int i = 0; i < findPathJobCount; i++)
            {
                FindPathJob findPathJob = new FindPathJob
                {
                    startPosition = new int2(0, 0),
                    endPosition = new int2(19, 19)
                };
                jobHandleArray[i] = findPathJob.Schedule();
            }

            JobHandle.CompleteAll(jobHandleArray);
            jobHandleArray.Dispose();

            Debug.Log($"Time: {(Time.realtimeSinceStartup - startTime) * 1000f}");
        }, 1f);
    }

    [BurstCompile]
    private struct FindPathJob : IJob {

        public int2 startPosition;
        public int2 endPosition;

        public void Execute()
        {
            int2 gridSize = new int2(20, 20);

            // Grid creation algorithm
            NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PathNode pathNode = new PathNode();
                    pathNode.x = x;
                    pathNode.y = y;

                    pathNode.index = CalculateIndex(x, y, gridSize.x);

                    pathNode.gCost = int.MaxValue;
                    pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
                    pathNode.CalculateFCost();

                    pathNode.isWalkable = true;
                    pathNode.cameFromNodeIndex = -1;

                    pathNodeArray[pathNode.index] = pathNode;
                }
            }
            // End grid creation algorithm
            
            // Set up some walls
            // {
            //     PathNode walkablePathNode = pathNodeArray[CalculateIndex(1, 0, gridSize.x)];
            //     walkablePathNode.SetIsWalkable(false);
            //     pathNodeArray[CalculateIndex(1, 0, gridSize.x)] = walkablePathNode;

            //     walkablePathNode = pathNodeArray[CalculateIndex(1, 1, gridSize.x)];
            //     walkablePathNode.SetIsWalkable(false);
            //     pathNodeArray[CalculateIndex(1, 1, gridSize.x)] = walkablePathNode;

            //     walkablePathNode = pathNodeArray[CalculateIndex(1, 2, gridSize.x)];
            //     walkablePathNode.SetIsWalkable(false);
            //     pathNodeArray[CalculateIndex(1, 2, gridSize.x)] = walkablePathNode;        
            // }

            // NativeArray<int2> neighborOffsetArray = new NativeArray<int2>(new int2[] {
            //     new int2(-1, 0),    // left
            //     new int2(1, 0),     // right
            //     new int2(0, 1),     // up
            //     new int2(0, -1),    // down
            //     new int2(-1, -1),   // left down
            //     new int2(-1, 1),    // left up
            //     new int2(1, 1),     // right up
            //     new int2(1, -1)     // right down
            // }, Allocator.Temp);

            int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);
            NativeArray<int2> neighborOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
            neighborOffsetArray[0] = new int2(-1, 0);   // left
            neighborOffsetArray[1] = new int2(1, 0);    // right
            neighborOffsetArray[2] = new int2(0, 1);    // up
            neighborOffsetArray[3] = new int2(0, -1);   // down
            neighborOffsetArray[4] = new int2(-1, -1);  // left down
            neighborOffsetArray[5] = new int2(-1, 1);   // left up
            neighborOffsetArray[6] = new int2(1, 1);    // right up
            neighborOffsetArray[7] = new int2(1, -1);   // right down

            PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            pathNodeArray[startNode.index] = startNode;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.index);

            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
                PathNode currentNode = pathNodeArray[currentNodeIndex];

                if (currentNodeIndex == endNodeIndex)
                {   
                    // We have reached our destination!
                    break;
                }

                // Remove current node from Open List
                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closedList.Add(currentNodeIndex);

                for (int i = 0; i < neighborOffsetArray.Length; i++)
                {
                    int2 neighborOffset = neighborOffsetArray[i];
                    int2 neighborPosition = new int2(currentNode.x + neighborOffset.x, currentNode.y + neighborOffset.y);

                    if (!IsPositionInsideGrid(neighborPosition, gridSize))
                    {  
                        // Neighbor not valid position
                        continue;
                    }

                    int neighborNodeIndex = CalculateIndex(neighborPosition.x, neighborPosition.y, gridSize.x);

                    if (closedList.Contains(neighborNodeIndex))
                    {
                        // Already searched this node
                        continue;
                    }

                    PathNode neighborNode = pathNodeArray[neighborNodeIndex];
                    if (!neighborNode.isWalkable)
                    {
                        continue;
                    }

                    int2 currentNodePosition = new int2(currentNode.x, currentNode.y);
                    
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighborPosition);
                    if (tentativeGCost < neighborNode.gCost)
                    {
                        neighborNode.cameFromNodeIndex = currentNodeIndex;
                        neighborNode.gCost = tentativeGCost;
                        neighborNode.CalculateFCost();
                        pathNodeArray[neighborNodeIndex] = neighborNode;

                        if (!openList.Contains(neighborNode.index))
                        {
                            openList.Add(neighborNode.index);
                        }
                    }
                }
            }

            PathNode endNode = pathNodeArray[endNodeIndex];
            if (endNode.cameFromNodeIndex == -1)
            {
                // We did not find a path
                //Debug.Log("Didn't find a path!");
            }
            else
            {
                // We found a path!
                NativeList<int2> path = CalculatePath(pathNodeArray, endNode);
                // foreach(int2 pathPos in path)
                // {
                //     Debug.Log(pathPos);
                // }
                path.Dispose();
            }

            // Free up memory here
            pathNodeArray.Dispose();
            openList.Dispose();
            closedList.Dispose();
            neighborOffsetArray.Dispose();
        }

        private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
        {
            if (endNode.cameFromNodeIndex == -1)
            {
                return new NativeList<int2>(Allocator.Temp);
            }
            else
            {
                NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
                path.Add(new int2(endNode.x, endNode.y));

                PathNode currentNode = endNode;
                while (currentNode.cameFromNodeIndex != -1)
                {
                    PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                    path.Add(new int2(cameFromNode.x, cameFromNode.y));
                    currentNode = cameFromNode;
                }

                return path;
            }
        }

        private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
        {
            return 
                gridPosition.x >= 0 &&
                gridPosition.y >= 0 &&
                gridPosition.x < gridSize.x &&
                gridPosition.y < gridSize.y;
        }

        private int CalculateIndex(int x, int y, int gridWidth)
        {
            return x + y * gridWidth;
        }

        private int CalculateDistanceCost(int2 aPos, int2 bPos)
        {
            int xDistance = math.abs(aPos.x - bPos.x);
            int yDistance = math.abs(aPos.y - bPos.y);
            int remaining = math.abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
        {
            PathNode lowestCostPathNode = pathNodeArray[openList[0]];
            for (int i = 0; i < openList.Length; i++)
            {
                PathNode testPathNode = pathNodeArray[openList[i]];
                if (testPathNode.fCost < lowestCostPathNode.fCost)
                {
                    lowestCostPathNode = testPathNode;
                }
            }
            return lowestCostPathNode.index;
        }

        private struct PathNode {
            // position
            public int x;
            public int y;

            // indices
            public int index;

            // A* Pathfinding variables (gCost = cost to move from start to next node, hCost = estimated cost to move from start to end, fCost = gCost + hCost)
            public int gCost;
            public int hCost;
            public int fCost;

            public bool isWalkable;

            // We need this to calculate the path from the end back to the start
            public int cameFromNodeIndex;

            public void CalculateFCost()
            {
                fCost = gCost + hCost;
            }

            public void SetIsWalkable(bool isWalkable)
            {
                this.isWalkable = isWalkable;
            }
        }
    }
}
