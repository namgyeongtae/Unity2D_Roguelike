using UnityEngine;

public class IdleState : State<Entity>
{
    public override void Enter()
    {
        Entity.State = EntityState.Idle;
    }
}
