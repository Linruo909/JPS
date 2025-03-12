using UnityEngine;
using System.Collections.Generic;

public class JPSAgent : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    public JPSPathfinder pathfinder;
    private List<Node> currentPath = new List<Node>();
    private int pathIndex;

    public LineRenderer pathLine; 
    public float lineHeightOffset = 0.5f;

    void Start()
    {
        // 获取Rigidbody组件，用来移动(处理碰撞)
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 鼠标点击触发寻路
        if (Input.GetMouseButtonDown(0))
        {
            // 重置路径状态
            currentPath?.Clear();
            pathIndex = 0;

            UpdatePathVisual(); // 路径可视化

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (pathfinder != null)
                {
                    // 返回路径List<Node>
                    currentPath = pathfinder.FindPath(transform.position, hit.point);
                    pathIndex = 0;
                }
                if (pathfinder == null)
                {
                    Debug.LogWarning("寻路器未初始化，请检查JPSInitializer配置");
                    return;
                }
                if (currentPath == null)
                {
                    //Debug.LogWarning("没有合适的路径");
                    return;
                }
            }
        }
        // DEBUG 模拟输入
        if (Input.GetKeyDown(KeyCode.K))
        {
            // 赋值targetPos
            Vector3 targetPos = new Vector3(1, 0, 0);
           
            currentPath = pathfinder.FindPath(transform.position, targetPos);
            Debug.Log($"targetPos设置为:{targetPos},且进入了Pathfinder");
        }
        // 根据路径移动
        if (currentPath != null && pathIndex < currentPath.Count)
        {
            Vector3 targetPos = currentPath[pathIndex].worldPosition;
            targetPos.y = transform.position.y; // 保持原有高度

            // 使用自带的rb组件进行移动
            rb.MovePosition(Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime));

            if (Vector3.Distance(transform.position, targetPos) < 0.2f)
            {
                pathIndex++;
            }
        }
    }

    // 路径可视化
    void UpdatePathVisual()
    {
        if (pathLine == null || currentPath == null) return;

        // 设置顶点位置
        Vector3[] positions = new Vector3[currentPath.Count];
        for (int i = 0; i < currentPath.Count; i++)
        {
            positions[i] = currentPath[i].worldPosition + Vector3.up * lineHeightOffset;
        }

        pathLine.positionCount = positions.Length;
        pathLine.SetPositions(positions);
    }

}