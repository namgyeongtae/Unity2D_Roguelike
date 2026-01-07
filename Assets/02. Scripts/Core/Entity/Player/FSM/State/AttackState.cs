using UnityEngine;

public class AttackState : State<Entity>
{
    private PlayerController _playerController;

    protected override void Setup()
    {
        _playerController = Entity.GetComponent<PlayerController>();
    }

    public override void Enter()
    {
        _playerController.State = EntityState.Attack;
    }

    public override void Exit()
    {
        
    }
}
