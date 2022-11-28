using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
using System;

public class Pathfinding : MonoBehaviour
{
    //public Transform seeker, target;
    PathRequestManager requestManager;
    Navigation nav;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        nav = GetComponent<Navigation>();
    }

    //private void Update()
    //{
    //    //if(Input.GetButtonDown("Jump"))
    //        FindPath(seeker, target);
    //}

    public void StartFindPath(Transform startTransform, Transform targetTransform)
    {
        StartCoroutine(FindPath(startTransform, targetTransform));
    }

    IEnumerator FindPath(Transform startTransform, Transform targetTransform)
    {
        //Stopwatch sw = new Stopwatch();
        //sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = nav.NodeFromWorldPoint(startTransform);
        Node targetNode = nav.NodeFromWorldPoint(targetTransform);

        if (startNode.walkable && targetNode.walkable)
        {
            List<Grid> suitableGrids = new List<Grid>();
            suitableGrids.Add(startNode.parentGrid);
            suitableGrids.Add(targetNode.parentGrid);

            Heap<Node> openSet = new Heap<Node>(nav.Size);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                //Node currentNode = openSet[0];

                //for (int i = 1; i < openSet.Count; i++)
                //{
                //    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                //    {
                //        currentNode = openSet[i];
                //    }
                //}

                //openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    //sw.Stop();
                    //print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in nav.GetNeighbours(currentNode, suitableGrids))
                {
                    if (neighbour == null) continue;

                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + TurbedGetDistance(currentNode, neighbour); // <- turbed here
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = TurbedGetDistance(neighbour, targetNode); // <- turbed here
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }
        //UnityEngine.Debug.Log("Could not find");
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;

    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(directionNew != directionOld)
            {
                waypoints.Add(path[i].spherePosActual);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    //int GetDistance(Node nodeA, Node nodeB)
    //{
    //    int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
    //    int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

    //    if (dstX > dstY)
    //        return 14 * dstY + 10 * (dstX - dstY);
    //    return 14 * dstX + 10 * (dstY - dstX);
    //}

    int TurbedGetDistance(Node nodeA, Node nodeB)
    {
        int dstX;
        int dstY;

        if (nodeA.parentGrid == nodeB.parentGrid)
        {
            dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        }
        else
        {
            Grid gridA = nodeA.parentGrid;
            Grid gridB = nodeB.parentGrid;
            int neighbourIndexGridB;

            Node edgeNodeA = GetEdgeNodeA(nodeA, gridB, out neighbourIndexGridB);

            int edgeX = Mathf.Abs(nodeA.gridX - edgeNodeA.gridX);
            int edgeY = Mathf.Abs(nodeA.gridY - edgeNodeA.gridY);

            Node edgeNodeB = GetEdgeNodeB(edgeNodeA, gridB, neighbourIndexGridB);

            int remainderX = Mathf.Abs(nodeB.gridX - edgeNodeB.gridX);
            int remainderY = Mathf.Abs(nodeB.gridY - edgeNodeB.gridY);

            dstX = edgeX + remainderY;
            dstY = edgeY + remainderX;
        }

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    Node GetEdgeNodeA(Node nodeA, Grid gridB, out int iGridB)
    {
        Grid gridA = nodeA.parentGrid;

        for (int i = 0; i < 4; i++)
        {
            if (gridA.adjacentGrids[i] == gridB)
            {
                iGridB = i;
                switch (i)
                {
                    case 0:
                        return gridA.grid[nodeA.gridX, 0];
                    case 1:
                        return gridA.grid[nodeA.gridX, 255];
                    case 2:
                        return gridA.grid[0, nodeA.gridY];
                    case 3:
                        return gridA.grid[255, nodeA.gridY];
                }
            }
        }

        UnityEngine.Debug.LogError("Could not find grid in adjacent grids. Pathfinding script.");
        iGridB = gridA.index;
        return nodeA;
    }

    Node GetEdgeNodeB(Node edgeNodeA, Grid gridB, int iGridB)
    {

        bool rotateClockwise = edgeNodeA.parentGrid.rotateClockwise[iGridB];

        if (rotateClockwise)
        {
            switch (iGridB)
            {
                case 0:
                    return gridB.grid[254, 255 - edgeNodeA.gridX];
                case 1:
                    return gridB.grid[1, 255 - edgeNodeA.gridX];
                case 2:
                    return gridB.grid[edgeNodeA.gridY, 1];
                case 3:
                    return gridB.grid[edgeNodeA.gridY, 254];
            }
        }
        else
        {
            switch (iGridB)
            {
                case 0:
                    return gridB.grid[1, edgeNodeA.gridX];
                case 1:
                    return gridB.grid[254, edgeNodeA.gridX];
                case 2:
                    return gridB.grid[255 - edgeNodeA.gridY, 254];
                case 3:
                    return gridB.grid[255 - edgeNodeA.gridY, 1];
            }
        }

        // should not happen
        UnityEngine.Debug.LogError("Could not get 90 or -90 angle between grids.");
        UnityEngine.Debug.Break();
        return edgeNodeA;

    }

}
