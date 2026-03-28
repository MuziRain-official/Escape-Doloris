using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Hunter : MonoBehaviour
{
    [Header("追踪设置")]
    public Transform target;           // 玩家transform

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 1f;  // 到达玩家附近停止
    }

    void Update()
    {
        if (target != null && agent != null)
        {
            agent.SetDestination(target.position);
        }
    }
}
