using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    public GameObject agentPrefab;
    public int spawnCount = 10;
    public int rangeX = 5;
    public int rangeY = 5;
    public float posZ = 1.02f;

    void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-rangeX, rangeX), posZ, Random.Range(-rangeY, rangeY));
            Instantiate(agentPrefab, pos, Quaternion.identity);
        }
    }
}