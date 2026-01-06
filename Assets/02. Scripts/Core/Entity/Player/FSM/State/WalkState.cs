using UnityEngine;

public class WalkState : State<Entity>
{
    private PlayerController _playerController;

    protected override void Setup()
    {
        _playerController = Entity.GetComponent<PlayerController>();
    }

    public override void Enter()
    {
        _playerController.State = EntityState.Walk;
    }

    public override void Exit()
    {

    }
}
