using UnityEngine;

public class WalkState : State<Entity>
{
    private EntityMovement entityMovement;

    protected override void Setup()
    {
        entityMovement = Entity.GetComponent<EntityMovement>();
    }

    public override void Enter()
    {
        Entity.State = EntityState.Walk;
    }

    public override void Exit()
    {

    }
}
