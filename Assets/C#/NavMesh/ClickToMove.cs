using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Camera mainCamera;
    public GameObject clickEffect;  // 拖入预制体

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            // 从摄像机发射射线到鼠标位置
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 检查目标点是否在NavMesh上
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
                {
                    agent.SetDestination(navHit.position);
                    // 生成特效并1秒后销毁
                    Destroy(Instantiate(clickEffect, navHit.position, Quaternion.identity), 1f);
                }
            }
        }
    }


}