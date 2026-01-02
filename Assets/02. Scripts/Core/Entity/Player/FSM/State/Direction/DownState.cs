using UnityEngine;

public class DownState : State<Entity>
{
    protected override void Setup()
    {

    }

    public override void Enter()
    {
        Entity.Direction = EntityDirection.Down;
    }

    public override void Exit()
    {

    }
}
