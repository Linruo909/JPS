using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Camera mainCamera;
    public GameObject clickEffect;  // ����Ԥ����

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        // ������������
        if (Input.GetMouseButtonDown(0))
        {
            // ��������������ߵ����λ��
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // ���Ŀ����Ƿ���NavMesh��
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
                {
                    agent.SetDestination(navHit.position);
                    // ������Ч��1�������
                    Destroy(Instantiate(clickEffect, navHit.position, Quaternion.identity), 1f);
                }
            }
        }
    }


}