using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{

    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColorGenerator colorGenerator = new ColorGenerator();

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    //storing values of inspector folded status
    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters; //for displaying terrain faces
    TerrainFace[] terrainFaces;

    //added
    GameObject[] meshObj = new GameObject[6];
    Grid[] vertexSphere = new Grid[6];

    private void OnValidate()//generating planet when changing settings
    {
        GeneratePlanet();
    }

    //initializing visible meshes
    void Initialize()
    {
        //Vector2 vectorA = Vector2.up;
        //Vector2 vectorB = Vector2.right;
        //Debug.Log(Vector2.SignedAngle(vectorA, vectorB));

        for (int i = 0; i < 6; i++)
        {
            vertexSphere[i] = new Grid(resolution, transform.GetChild(i).gameObject, i);
        }

        MapFaces();

        shapeGenerator.UpdateSettings(shapeSettings);
        colorGenerator.UpdateSettings(colorSettings);

        if (meshFilters == null || meshFilters.Length == 0) //do not regenerate new mesh
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };//for corrent facing terrain faces

        for (int i = 0; i < 6; i++) //creating game objects for planet faces
        {
            if (meshFilters[i] == null) //generate mesh if there is none
            {
                /*GameObject*/ meshObj[i] = new GameObject("mesh");
                meshObj[i].transform.parent = transform;

                meshObj[i].AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj[i].AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

            //Debug.Log(vertexSphere[0].relatedMeshObj);

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i], vertexSphere[i]);
        }
    }
    public void GeneratePlanet() //when generating new planet
    {
        Initialize();
        GenerateMesh();
        GenerateColor();

    }

    public void OnShapeSettingsUpdated() //when shape settings updated
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColorSettingsUpdated() //when color settings updated
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColor();
        }
    }

    void GenerateMesh() //constructing mesh
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }
    void GenerateColor() //applying color settings
    {
        colorGenerator.UpdateColors();
    }

    public Grid[] GetVertexSphere()
    {
        for (int i = 0; i < 6; i++)
        {
            //vertexSphere[i] = new Grid(resolution, transform.GetChild(i).gameObject);
            vertexSphere[i].grid = terrainFaces[i].grid;
        }

        return vertexSphere;
    }

    void MapFaces()
    {

        int[,] faceMap = new int[,] {
            { 4, 5, 2, 3 },
            { 4, 5, 3, 2 },
            { 0, 1, 4, 5 },
            { 0, 1, 5, 4 },
            { 3, 2, 1, 0 },
            { 3, 2, 0, 1 }
        };

        bool[,] rotateClockwise = new bool[,]
        {
            { true, true, true, false },
            { false, false, false, true },
            { false, false, false, true },
            { true, true, true, false },
            { true, true, true, false },
            { false, false, false, true }
        };

        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                vertexSphere[i].adjacentGrids[j] = vertexSphere[faceMap[i, j]];
                vertexSphere[i].adjacentGrids[j] = vertexSphere[faceMap[i, j]];
                vertexSphere[i].adjacentGrids[j] = vertexSphere[faceMap[i, j]];
                vertexSphere[i].adjacentGrids[j] = vertexSphere[faceMap[i, j]];

                vertexSphere[i].rotateClockwise[j] = rotateClockwise[i, j];
                vertexSphere[i].rotateClockwise[j] = rotateClockwise[i, j];
                vertexSphere[i].rotateClockwise[j] = rotateClockwise[i, j];
                vertexSphere[i].rotateClockwise[j] = rotateClockwise[i, j];
            }
        }

        //vertexSphere[0].orientation = Vector2.up;
        //vertexSphere[1].orientation = Vector2.down;
        //vertexSphere[2].orientation = Vector2.right;
        //vertexSphere[3].orientation = Vector2.left;
        //vertexSphere[4].orientation = Vector2.right;
        //vertexSphere[5].orientation = Vector2.right;
    }

    public float getPlanetRadius()
    {
        return shapeSettings.planetRadius;
    }
}