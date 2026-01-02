using UnityEngine;

public class DeadState : State<Entity>
{
    protected override void Setup()
    {

    }

    public override void Enter()
    {   
        Entity.State = EntityState.Dead;
    }

    public override void Exit()
    {
        
    }
}
