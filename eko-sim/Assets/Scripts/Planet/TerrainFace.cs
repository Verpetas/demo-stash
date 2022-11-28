using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    ShapeGenerator shapeGenerator;
    Mesh mesh; //one of six generated meshes
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    // added
    [HideInInspector] Vector3 vertexNormal;
    [HideInInspector] public Node[,] grid;
    Grid parentGrid;

    //terrainface constructor
    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp, Grid parentGrid)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        //local face axis
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);

        //added
        this.parentGrid = parentGrid;
    }

    // flat mesh constructing method
    public void ConstructMesh()
    {
        // added
        float planetRadius = GameObject.Find("Planet").GetComponent<Planet>().getPlanetRadius();
        grid = new Node[resolution, resolution];

        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1); //completion percentage
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB; //global point position
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized; //making all points the same distance from the center

                // added
                vertexNormal = pointOnUnitSphere * planetRadius;

                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere); //creating bumps

                // added
                if (((x == 0 || x == resolution-1) && (y == 0 || y == resolution-1)) || (vertices[i]-vertexNormal).magnitude > 10) // setting unwalkable terrain
                {
                    grid[x, y] = new Node(vertexNormal, vertices[i], false, x, y, parentGrid);
                }
                else
                    grid[x, y] = new Node(vertexNormal, vertices[i], true, x, y, parentGrid);

                if (x != resolution - 1 && y != resolution - 1)
                {
                    //creating triangles
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}