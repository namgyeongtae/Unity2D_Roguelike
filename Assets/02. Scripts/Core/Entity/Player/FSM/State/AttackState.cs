using UnityEngine;

public class AttackState : State<Entity>
{
    protected override void Setup()
    {

    }

    public override void Enter()
    {
        Entity.State = EntityState.Attack;
    }
}
