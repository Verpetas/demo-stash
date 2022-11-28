using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    public LayerMask groundMask;

    Planet planetScript;
    Grid[] vertexSphere;
    float planetRadius;

    public int vertexResolution;

    // Start is called before the first frame update
    void Awake()
    {
        planetScript = GameObject.Find("Planet").GetComponent<Planet>();
        CollectVertexSphere();
        vertexResolution = planetScript.resolution;

        planetRadius = planetScript.getPlanetRadius();
    }

    private void Start()
    {
        //CollectVertexSphere();
    }

    public List<Node> GetNeighbours(Node node, List<Grid> suitableGrids)
    {
        List<Node> neighbors = new List<Node>();
        Grid parentGrid = node.parentGrid;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (!suitableGrids.Contains(node.parentGrid))
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < vertexResolution /* gridSizeX */ && checkY >= 0 && checkY < vertexResolution /* gridSizeY */)
                {
                    neighbors.Add(node.parentGrid.grid[checkX, checkY]);
                }
                else if (!((checkX < 0 || checkX >= vertexResolution) && (checkY < 0 || checkY >= vertexResolution))) // check if not on corner
                {
                    if (checkX < 0)
                    {
                        neighbors.Add(ConvertNode(parentGrid.grid[0, checkY], parentGrid.adjacentGrids[2]));
                    }
                    else if (checkX >= vertexResolution)
                    {
                        neighbors.Add(ConvertNode(parentGrid.grid[vertexResolution-1, checkY], parentGrid.adjacentGrids[3]));
                    }
                    else if (checkY < 0)
                    {
                        neighbors.Add(ConvertNode(parentGrid.grid[checkX, 0], parentGrid.adjacentGrids[0]));
                    }
                    else if (checkY >= vertexResolution)
                    {
                        neighbors.Add(ConvertNode(parentGrid.grid[checkX, vertexResolution-1], parentGrid.adjacentGrids[1]));
                    }
                }
                else
                    Debug.Log("Corner reached. " + checkX + ", " + checkY);
            }
        }

        return neighbors;
    }

    public Node NodeFromWorldPoint(Transform worldPos)
    {
        Grid grid = GetGrid(worldPos);

        Vector3 normalWorldPos = worldPos.position.normalized * planetRadius;

        float currDistance = Mathf.Infinity;
        float prevDistance = Mathf.Infinity;

        int xRow = vertexResolution - 1;
        int yRow = vertexResolution - 1;

        for (int x = 0; x < vertexResolution; x++)
        {
            currDistance = Vector3.Distance(normalWorldPos, grid.grid[x, vertexResolution/2].spherePosNormal);

            if(currDistance > prevDistance)
            {
                xRow = x - 1;
                break;
            }

            prevDistance = currDistance;
        }

        currDistance = Mathf.Infinity;
        prevDistance = Mathf.Infinity;

        for (int y = 0; y < vertexResolution; y++)
        {
            currDistance = Vector3.Distance(normalWorldPos, grid.grid[xRow, y].spherePosNormal);

            if (currDistance > prevDistance)
            {
                yRow = y - 1;
                break;
            }

            prevDistance = currDistance;
        }

        return grid.grid[xRow, yRow];
    }

    void CollectVertexSphere()
    {
        vertexSphere = planetScript.GetVertexSphere();
    }

    public int Size
    {
        get { return vertexResolution * vertexResolution; }
    }

    Node ConvertNode(Node edgeNodeA, Grid gridB)
    {
        Grid gridA = edgeNodeA.parentGrid;
        bool? rotateClockwise = null;
        int iGridB = 5; // outside the bounds

        for (int i = 0; i < 4; i++)
        {
            if (gridA.adjacentGrids[i] == gridB)
            {
                rotateClockwise = gridA.rotateClockwise[i];
                iGridB = i;
            }   
        }

        //float facesAngle = Vector2.SignedAngle(gridB.orientation, edgeNodeA.parentGrid.orientation);
        if(rotateClockwise != null)
        {
            if (rotateClockwise == true)
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
        }

        // should not happen
        Debug.LogError("Value rotateClockwise not assigned.");
        return null;
    }

    Grid GetGrid(Transform worldPos)
    {
        RaycastHit hit;
        GameObject face;

        Physics.Raycast(worldPos.position, -worldPos.up, out hit, 500f, groundMask);

        if (hit.collider != null)
        {
            face = hit.collider.gameObject;

            for (int i = 0; i < 6; i++)
            {
                if (face == vertexSphere[i].relatedMeshObj)
                    return vertexSphere[i];
            }
        }

        Debug.LogError("Could not get grid.");
        return null;
    }

    //public List<Node> path;
    void OnDrawGizmos()
    {
        //if (vertexSphere != null)
        //{
        //    foreach (Grid grid in vertexSphere)
        //    {
        //        foreach (Node n in grid.grid)
        //        {
        //            //Gizmos.color = (n.walkable) ? Color.white : Color.red;
        //            //if (path != null)
        //            //    if (path.Contains(n))
        //            //        Gizmos.DrawCube(n.spherePosActual, Vector3.one);
        //            if(n.walkable == false)
        //                Gizmos.DrawSphere(n.spherePosActual, 1);
        //        }
        //    }
        //}

        if (vertexSphere != null)
        {
            foreach (Grid face in vertexSphere)
            {
                Gizmos.DrawSphere(face.grid[1, 2].spherePosActual, 1);
                Gizmos.DrawSphere(face.grid[2, 2].spherePosActual, 1);
                Gizmos.DrawSphere(face.grid[3, 2].spherePosActual, 1);
            }
        }
    }
}
