using UnityEngine;
using UnityEngine.AI;

public enum PlayerState
{
    Idle,
    Move
}

public class NavMesh : MonoBehaviour
{
    public NavMeshAgent agent;
    public PlayerState currentState = PlayerState.Idle;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        UpdateState();

        if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    void UpdateState()
    {
        PlayerState newState = agent.velocity.magnitude > 0.1f ? PlayerState.Move : PlayerState.Idle;

        if (newState != currentState)
        {
            OnStateExit(currentState);
            currentState = newState;
            OnStateEnter(currentState);
        }
    }

    void OnStateEnter(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                // 可以在这里播放 idle 动画
                break;
            case PlayerState.Move:
                // 可以在这里播放移动动画
                break;
        }
    }

    void OnStateExit(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Move:
                break;
        }
    }
}
