using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class StateMachine : MonoBehaviour
{
    public EntityState currentState { get; private set; }

    public void Initialize(EntityState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(EntityState newState)
    {
        currentState?.Exit();

        currentState = newState;

        currentState?.Enter();

        DebugManager.Log($"Transitioning from {currentState} to {newState}");
    }

    public void UpdateActiveState()
    {
        currentState?.Update();
    }
}
