using UnityEngine;

public class UpState : State<Entity>
{
    private EntityMovement entityMovement;

    protected override void Setup()
    {
        entityMovement = Entity.GetComponent<EntityMovement>();
    }

    public override void Enter()
    {
        Entity.Direction = EntityDirection.Up;
    }
    
    public override void Exit()
    {

    }
}
