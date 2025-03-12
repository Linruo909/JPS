using UnityEngine;

public class JPSInitializer : MonoBehaviour
{
    [Header("前置")]
    public JPSGrid grid;
    public JPSAgent[] agents;

    void Start()
    {
        // 自动查找所有代理
        JPSAgent[] agents = FindObjectsOfType<JPSAgent>();
        // 初始化
        JPSPathfinder pathfinder = new JPSPathfinder(grid);

        // 为所有代理分配寻路逻辑
        foreach (JPSAgent agent in agents)
        {
            agent.pathfinder = pathfinder;
        }

        Debug.Log($"JPS系统已初始化，共管理{agents.Length}个代理");
    }
}