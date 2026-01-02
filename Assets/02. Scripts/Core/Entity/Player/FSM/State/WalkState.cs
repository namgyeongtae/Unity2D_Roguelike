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
        
    }

    public override void Exit()
    {

    }
}
