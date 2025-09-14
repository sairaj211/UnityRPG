using UnityEngine;

public class EntityState 
{
    protected StateMachine stateMachine;

    public EntityState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
}
