using UnityEngine;
using System.Collections.Generic;

public class JPSGrid : MonoBehaviour
{
    public LayerMask obstacleMask;
    public Vector2 gridWorldSize;
    public float nodeRadius = 0.5f;
    public Node[,] grid;

    public int gridSizeX;
    public int gridSizeY;

    public int MaxSize => gridSizeX * gridSizeY;

    void Start() => CreateGrid();
    
    // ��������
    public void CreateGrid()
    {
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / (nodeRadius * 2));
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / (nodeRadius * 2));
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft +
                    Vector3.right * (x * nodeRadius * 2 + nodeRadius) +
                    Vector3.forward * (y * nodeRadius * 2 + nodeRadius);
                // ͨ���Ҹ�����layer����ͨ�����ж�(ʶ���ϰ���)
                bool walkable = !Physics.CheckSphere(
                    worldPoint,
                    nodeRadius,
                    obstacleMask,
                    QueryTriggerInteraction.Ignore // ���Դ�����
                );
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }
    // ����ת��
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y; // ʵ������z��
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.FloorToInt((gridSizeX) * percentX);
        int y = Mathf.FloorToInt((gridSizeY) * percentY);
        return grid[x, y];
    }

    public Node GetNode(int x, int y)
    {
        // �߽���
        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
            return null;
        return grid[x, y];
    }

    // gizmos���ӻ�
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = n.walkable ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeRadius * 1.9f));
            }
        }
    }
}