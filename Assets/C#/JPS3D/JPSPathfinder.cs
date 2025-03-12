using UnityEngine;
using System.Collections.Generic;

public class JPSPathfinder
{
    private JPSGrid grid;
    public JPSPathfinder(JPSGrid grid) => this.grid = grid;

    private List<Node> pathCache = new List<Node>();





    // Ѱ·����
    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // ת������������
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        //Debug.Log($"����Ѱ·:{startNode},{targetNode}");

        // ��ͨ���ж�
        if (!startNode.walkable || !targetNode.walkable)
            return new List<Node>();
        // ��ʼ��
        List<Node> openSet = new List<Node>(grid.MaxSize); // �����б� �滹û̽����Ľڵ�
        HashSet<Node> closedSet = new HashSet<Node>(); // �ر��б���û���õĽڵ�
        openSet.Add(startNode); // ��ʼ�ڵ�

        while (openSet.Count > 0)
        {
            // ȡfCost��͵Ľڵ�,ѡ����Ϊ�����㣬��ʼλ��startPos
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }


            // �����ǰ�ڵ���Ŀ��ڵ㣬����·�����൱���Ѿ������ˣ���ʼ����·��(ͨ��parent)
            if (currentNode == targetNode)
            {
                //Debug.Log("===�ɹ��ҵ�Ŀ��ڵ�===");
                List<Node> path = new List<Node>();
                Node temp = currentNode;
                while (temp != startNode)
                {
                    path.Add(temp);
                    temp = temp.parent ?? startNode;    // ��ֹ������
                }
                path.Reverse();
                return path;    // ���ս��
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // ��������ھӽڵ�
            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;   // �ϰ����ж�

                int tentativeGCost = currentNode.gCost + CalculateGCost(currentNode, neighbor);

                if (tentativeGCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = CalculateHCost(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return null; // ���û��·��
    }







    // ��ȡ�ھӽڵ�
    private List<Node> GetNeighbors(Node node)
    {
        
        List<Node> neighbors = new List<Node>();

        // �����Լ������˸����� 3x3-1
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;  // �����Լ�

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // ȷ���ھ�������Χ��
                if (checkX >= 0 && checkX < grid.gridSizeX && checkY >= 0 && checkY < grid.gridSizeY)
                {
                    neighbors.Add(grid.grid[checkX, checkY]);
                    //Debug.Log($"�����һ���ھ�({checkX},{checkY})");     
                }
            }
            
        }
        //Debug.Log($"���ڻ�ȡ:{node.gridX},{node.gridY}���ھӽڵ�");
        return neighbors;
    }

    // �����ƶ���GCost������ʼ�ڵ㵽��ǰ�ڵ��ʵ�ʴ��ۣ�
    private int CalculateGCost(Node a, Node b)
    {
        // �����б�߷��򣬴��۽ϸ�
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        if (dstX == 1 && dstY == 1) // б��
            return 14; // б�ߴ���ͨ���ǡ�2 �� 1.41����������Ϊ14����ֱ���Ըߣ�

        return 10; // ֱ�߷���
    }

    // ����HCost������ʽ��ͨ���������پ������ŷ����þ��룩
    private int CalculateHCost(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);
        return (dstX + dstY) * 10; // �����پ������10��ΪHCost
    }

   

















    //public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    //{
    //    pathCache.Clear();
    //    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    //    sw.Start();

    //    Node startNode = grid.NodeFromWorldPoint(startPos);
    //    Node targetNode = grid.NodeFromWorldPoint(targetPos);
    //    Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
    //    HashSet<Node> closedSet = new HashSet<Node>();
    //    openSet.Add(startNode);

    //    while (openSet.Count > 0)
    //    {
    //        Node currentNode = openSet.RemoveFirst();
    //        closedSet.Add(currentNode);

    //        if (currentNode == targetNode)
    //            return RetracePath(startNode, targetNode);

    //        foreach (Node neighbour in GetPrunedNeighbours(currentNode))
    //        {
    //            if (closedSet.Contains(neighbour)) continue;

    //            Node jumpPoint = Jump(neighbour, currentNode, targetNode);
    //            if (jumpPoint == null) continue;

    //            int newCost = currentNode.gCost + GetDistance(currentNode, jumpPoint);
    //            if (newCost < jumpPoint.gCost || !openSet.Contains(jumpPoint))
    //            {
    //                jumpPoint.gCost = newCost;
    //                jumpPoint.hCost = GetDistance(jumpPoint, targetNode);
    //                jumpPoint.parent = currentNode;

    //                if (!openSet.Contains(jumpPoint))
    //                    openSet.Add(jumpPoint);
    //            }
    //        }
    //    }
    //    sw.Stop();
    //    Debug.Log($"Ѱ·��ʱ��{sw.ElapsedMilliseconds}ms");

    //    return new List<Node>(pathCache); // ���ظ���
    //}

    // ������
    private Node Jump(Node current, Node parent, Node target, int depth = 0)
    {
        // �ݹ�������ƣ���ֹջ�����
        const int maxDepth = 1000;
        if (current == null || depth > maxDepth) return null;

        int dx = current.gridX - parent.gridX;
        int dy = current.gridY - parent.gridY;

        // ����Ŀ��ڵ�ʱ��������
        if (current == target) return current;

        // ǿ���ھӼ��
        if (HasForcedNeighbour(current, dx, dy))
            return current;

        // �Խ�����Ծ
        if (dx != 0 && dy != 0)
        {
            // ��ֵ���
            Node horizontal = grid.GetNode(current.gridX + dx, current.gridY);
            Node vertical = grid.GetNode(current.gridX, current.gridY + dy);

            if (Jump(horizontal, current, target, depth + 1) != null ||
                Jump(vertical, current, target, depth + 1) != null)
                return current;
        }

        // ��������Ծ���߽��飩
        Node next = grid.GetNode(current.gridX + dx, current.gridY + dy);
        return next != null && next.walkable ? Jump(next, current, target, depth + 1) : null;
    }

    // ǿ���ھ�
    private bool HasForcedNeighbour(Node node, int dx, int dy)
    {
        // ˮƽ/��ֱ������
        if (dx == 0 || dy == 0)
        {
            int perpDx = dy, perpDy = dx; // ��ֱ����
            Node n1 = grid.GetNode(node.gridX + perpDx, node.gridY + perpDy);
            Node n2 = grid.GetNode(node.gridX - perpDx, node.gridY - perpDy);
            return (n1 != null && !n1.walkable) || (n2 != null && !n2.walkable);
        }
        // �Խ��߷�����
        else
        {
            Node n1 = grid.GetNode(node.gridX - dx, node.gridY);
            Node n2 = grid.GetNode(node.gridX, node.gridY - dy);
            return (n1 != null && !n1.walkable) || (n2 != null && !n2.walkable);
        }
    }
    // ·������
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        int safetyCounter = 0;
        const int maxPathLength = 1000; // ������һ����󳤶ȣ���ֹ��bug�ڴ汬ը

        while (currentNode != null && currentNode != startNode)
        {
            // ��ֹ����ѭ��
            if (safetyCounter++ > maxPathLength)
            {
                Debug.LogWarning("·�����ݳ�����󳤶ȣ����ܴ���ѭ������");
                break;
            }

            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        if (currentNode == startNode)
        {
            path.Reverse();
        }
        else
        {
            path.Clear(); // ���ؿ�·����ʾʧ��
        }
        return path;
    }

    // �������
    private int GetDistance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        return 10 * (dx + dy) + (14 - 2 * 10) * Mathf.Min(dx, dy);
    }

    // ��ȡ�ü��ھ�
    public List<Node> GetPrunedNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                Node n = grid.GetNode(node.gridX + x, node.gridY + y);
                if (n != null && n.walkable)
                    neighbours.Add(n);
            }
        }
        return neighbours;
    }
}