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
        // ��ȡRigidbody����������ƶ�(������ײ)
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // ���������Ѱ·
        if (Input.GetMouseButtonDown(0))
        {
            // ����·��״̬
            currentPath?.Clear();
            pathIndex = 0;

            UpdatePathVisual(); // ·�����ӻ�

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (pathfinder != null)
                {
                    // ����·��List<Node>
                    currentPath = pathfinder.FindPath(transform.position, hit.point);
                    pathIndex = 0;
                }
                if (pathfinder == null)
                {
                    Debug.LogWarning("Ѱ·��δ��ʼ��������JPSInitializer����");
                    return;
                }
                if (currentPath == null)
                {
                    //Debug.LogWarning("û�к��ʵ�·��");
                    return;
                }
            }
        }
        // DEBUG ģ������
        if (Input.GetKeyDown(KeyCode.K))
        {
            // ��ֵtargetPos
            Vector3 targetPos = new Vector3(1, 0, 0);
           
            currentPath = pathfinder.FindPath(transform.position, targetPos);
            Debug.Log($"targetPos����Ϊ:{targetPos},�ҽ�����Pathfinder");
        }
        // ����·���ƶ�
        if (currentPath != null && pathIndex < currentPath.Count)
        {
            Vector3 targetPos = currentPath[pathIndex].worldPosition;
            targetPos.y = transform.position.y; // ����ԭ�и߶�

            // ʹ���Դ���rb��������ƶ�
            rb.MovePosition(Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime));

            if (Vector3.Distance(transform.position, targetPos) < 0.2f)
            {
                pathIndex++;
            }
        }
    }

    // ·�����ӻ�
    void UpdatePathVisual()
    {
        if (pathLine == null || currentPath == null) return;

        // ���ö���λ��
        Vector3[] positions = new Vector3[currentPath.Count];
        for (int i = 0; i < currentPath.Count; i++)
        {
            positions[i] = currentPath[i].worldPosition + Vector3.up * lineHeightOffset;
        }

        pathLine.positionCount = positions.Length;
        pathLine.SetPositions(positions);
    }

}