using UnityEngine;

public class DownState : State<Entity>
{
    private PlayerController _playerController;
    protected override void Setup()
    {
        _playerController = Entity.GetComponent<PlayerController>();
    }

    public override void Enter()
    {
       _playerController.Direction = EntityDirection.Down;
    }

    public override void Exit()
    {

    }
}
