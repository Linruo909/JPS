using UnityEngine;

public class JPSInitializer : MonoBehaviour
{
    [Header("ǰ��")]
    public JPSGrid grid;
    public JPSAgent[] agents;

    void Start()
    {
        // �Զ��������д���
        JPSAgent[] agents = FindObjectsOfType<JPSAgent>();
        // ��ʼ��
        JPSPathfinder pathfinder = new JPSPathfinder(grid);

        // Ϊ���д������Ѱ·�߼�
        foreach (JPSAgent agent in agents)
        {
            agent.pathfinder = pathfinder;
        }

        Debug.Log($"JPSϵͳ�ѳ�ʼ����������{agents.Length}������");
    }
}