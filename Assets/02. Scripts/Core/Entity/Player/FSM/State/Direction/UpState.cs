using UnityEngine;

public class UpState : State<Entity>
{
    private PlayerController _playerController;

    protected override void Setup()
    {
        _playerController = Entity.GetComponent<PlayerController>();
    }

    public override void Enter()
    {
        _playerController.Direction = EntityDirection.Up;
    }
    
    public override void Exit()
    {

    }
}
