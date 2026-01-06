using UnityEngine;

public class DeadState : State<Entity>
{
    private PlayerController _playerController;
    protected override void Setup()
    {

    }

    public override void Enter()
    {   
        _playerController.State = EntityState.Dead;
    }

    public override void Exit()
    {
        
    }
}
