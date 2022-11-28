using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public Node[,] grid;
	public GameObject relatedMeshObj;

	public Grid[] adjacentGrids = new Grid[4];
	public bool[] rotateClockwise = new bool[4];
	public int index;
	//public Vector2 orientation;

	public Grid(int resolution, GameObject relatedMeshObj, int index)
	{
		grid = new Node[resolution, resolution];
		this.relatedMeshObj = relatedMeshObj;
		this.index = index;
	}

}
