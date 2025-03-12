using UnityEngine;
using System;

[System.Serializable]
public class Node : IComparable<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX, gridY;
    public int gCost, hCost;
    public Node parent;

    public Node(bool walkable, Vector3 worldPos, int x, int y)
    {
        this.walkable = walkable;
        this.worldPosition = worldPos;
        this.gridX = x;
        this.gridY = y;
    }

    public int fCost => gCost + hCost;

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        return compare == 0 ? hCost.CompareTo(other.hCost) : compare;
    }
}