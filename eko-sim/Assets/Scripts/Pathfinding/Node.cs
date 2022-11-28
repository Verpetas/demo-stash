using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>
{
	public Vector3 spherePosNormal;
	public Vector3 spherePosActual;

	public bool walkable;
	//public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public Node parent;
	int heapIndex;

	public Grid parentGrid;

	public Node(Vector3 _spherePosNormal, Vector3 _spherePosActual, bool _walkable, /*Vector3 _worldPos,*/ int _gridX, int _gridY, Grid _parentGrid)
	{
		spherePosNormal = _spherePosNormal;
		spherePosActual = _spherePosActual;
		walkable = _walkable;
		//worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;

		parentGrid = _parentGrid;
	}

	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}

	public int HeapIndex
    {
        get
        {
			return heapIndex;
        }
        set
        {
			heapIndex = value;
        }
    }

	public int CompareTo(Node nodeToCompare)
    {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0)
        {
			compare = hCost.CompareTo(nodeToCompare.hCost);
        }
		return -compare;
    }
}