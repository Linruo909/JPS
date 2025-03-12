using UnityEngine;
using System.Collections.Generic;

public class JPSPathfinder
{
    private JPSGrid grid;
    public JPSPathfinder(JPSGrid grid) => this.grid = grid;

    private List<Node> pathCache = new List<Node>();





    // 寻路方法
    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // 转换世界坐标先
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        //Debug.Log($"正在寻路:{startNode},{targetNode}");

        // 可通行判断
        if (!startNode.walkable || !targetNode.walkable)
            return new List<Node>();
        // 初始化
        List<Node> openSet = new List<Node>(grid.MaxSize); // 启动列表 存还没探索完的节点
        HashSet<Node> closedSet = new HashSet<Node>(); // 关闭列表，存没有用的节点
        openSet.Add(startNode); // 起始节点

        while (openSet.Count > 0)
        {
            // 取fCost最低的节点,选来作为搜索点，起始位置startPos
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }


            // 如果当前节点是目标节点，构建路径，相当于已经找完了，开始回溯路径(通过parent)
            if (currentNode == targetNode)
            {
                //Debug.Log("===成功找到目标节点===");
                List<Node> path = new List<Node>();
                Node temp = currentNode;
                while (temp != startNode)
                {
                    path.Add(temp);
                    temp = temp.parent ?? startNode;    // 防止空引用
                }
                path.Reverse();
                return path;    // 最终结果
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // 检查所有邻居节点
            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;   // 障碍物判断

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
        return null; // 如果没有路径
    }







    // 获取邻居节点
    private List<Node> GetNeighbors(Node node)
    {
        
        List<Node> neighbors = new List<Node>();

        // 除了自己，共八个方向 3x3-1
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;  // 忽略自己

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // 确保邻居在网格范围内
                if (checkX >= 0 && checkX < grid.gridSizeX && checkY >= 0 && checkY < grid.gridSizeY)
                {
                    neighbors.Add(grid.grid[checkX, checkY]);
                    //Debug.Log($"添加了一个邻居({checkX},{checkY})");     
                }
            }
            
        }
        //Debug.Log($"正在获取:{node.gridX},{node.gridY}的邻居节点");
        return neighbors;
    }

    // 计算移动的GCost（从起始节点到当前节点的实际代价）
    private int CalculateGCost(Node a, Node b)
    {
        // 如果是斜线方向，代价较高
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        if (dstX == 1 && dstY == 1) // 斜线
            return 14; // 斜线代价通常是√2 ≈ 1.41，四舍五入为14（比直线略高）

        return 10; // 直线方向
    }

    // 计算HCost（启发式，通常是曼哈顿距离或者欧几里得距离）
    private int CalculateHCost(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);
        return (dstX + dstY) * 10; // 曼哈顿距离乘以10作为HCost
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
    //    Debug.Log($"寻路耗时：{sw.ElapsedMilliseconds}ms");

    //    return new List<Node>(pathCache); // 返回副本
    //}

    // 跳点检测
    private Node Jump(Node current, Node parent, Node target, int depth = 0)
    {
        // 递归深度限制（防止栈溢出）
        const int maxDepth = 1000;
        if (current == null || depth > maxDepth) return null;

        int dx = current.gridX - parent.gridX;
        int dy = current.gridY - parent.gridY;

        // 到达目标节点时立即返回
        if (current == target) return current;

        // 强制邻居检测
        if (HasForcedNeighbour(current, dx, dy))
            return current;

        // 对角线跳跃
        if (dx != 0 && dy != 0)
        {
            // 空值检查
            Node horizontal = grid.GetNode(current.gridX + dx, current.gridY);
            Node vertical = grid.GetNode(current.gridX, current.gridY + dy);

            if (Jump(horizontal, current, target, depth + 1) != null ||
                Jump(vertical, current, target, depth + 1) != null)
                return current;
        }

        // 主方向跳跃（边界检查）
        Node next = grid.GetNode(current.gridX + dx, current.gridY + dy);
        return next != null && next.walkable ? Jump(next, current, target, depth + 1) : null;
    }

    // 强迫邻居
    private bool HasForcedNeighbour(Node node, int dx, int dy)
    {
        // 水平/垂直方向检测
        if (dx == 0 || dy == 0)
        {
            int perpDx = dy, perpDy = dx; // 垂直方向
            Node n1 = grid.GetNode(node.gridX + perpDx, node.gridY + perpDy);
            Node n2 = grid.GetNode(node.gridX - perpDx, node.gridY - perpDy);
            return (n1 != null && !n1.walkable) || (n2 != null && !n2.walkable);
        }
        // 对角线方向检测
        else
        {
            Node n1 = grid.GetNode(node.gridX - dx, node.gridY);
            Node n2 = grid.GetNode(node.gridX, node.gridY - dy);
            return (n1 != null && !n1.walkable) || (n2 != null && !n2.walkable);
        }
    }
    // 路径回溯
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        int safetyCounter = 0;
        const int maxPathLength = 1000; // 内置了一个最大长度，防止出bug内存爆炸

        while (currentNode != null && currentNode != startNode)
        {
            // 防止无限循环
            if (safetyCounter++ > maxPathLength)
            {
                Debug.LogWarning("路径回溯超过最大长度，可能存在循环引用");
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
            path.Clear(); // 返回空路径表示失败
        }
        return path;
    }

    // 距离计算
    private int GetDistance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        return 10 * (dx + dy) + (14 - 2 * 10) * Mathf.Min(dx, dy);
    }

    // 获取裁剪邻居
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