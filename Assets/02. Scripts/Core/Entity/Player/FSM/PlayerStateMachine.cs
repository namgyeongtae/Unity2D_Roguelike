using UnityEngine;

public enum EntityStateMachineLayer
{
    DIR = 0,
    STATE = 1,

}

public class PlayerStateMachine : MonoStateMachine<Entity>
{
    private PlayerController _playerController;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    protected override void AddStates()
    {
        AddState<DownState>((int)EntityStateMachineLayer.DIR);
        AddState<UpState>((int)EntityStateMachineLayer.DIR);
        AddState<LeftState>((int)EntityStateMachineLayer.DIR);
        AddState<RightState>((int)EntityStateMachineLayer.DIR);

        AddState<IdleState>((int)EntityStateMachineLayer.STATE);
        AddState<WalkState>((int)EntityStateMachineLayer.STATE);
        AddState<DashState>((int)EntityStateMachineLayer.STATE);
        AddState<DeadState>((int)EntityStateMachineLayer.STATE);
        AddState<AttackState>((int)EntityStateMachineLayer.STATE);
    }

    protected override void MakeTransitions()
    {
        MakeAnyTransition<UpState>(state => _playerController.Direction == EntityDirection.Up, (int)EntityStateMachineLayer.DIR);
        MakeAnyTransition<DownState>(state => _playerController.Direction == EntityDirection.Down, (int)EntityStateMachineLayer.DIR);
        MakeAnyTransition<LeftState>(state => _playerController.Direction == EntityDirection.Left, (int)EntityStateMachineLayer.DIR);
        MakeAnyTransition<RightState>(state => _playerController.Direction == EntityDirection.Right, (int)EntityStateMachineLayer.DIR);


        MakeTransition<IdleState, WalkState>(state => Owner.Movement.MoveDir != Vector2.zero, (int)EntityStateMachineLayer.STATE);
        MakeTransition<WalkState, IdleState>(state => Owner.Movement.MoveDir == Vector2.zero, (int)EntityStateMachineLayer.STATE);
        MakeTransition<AttackState, IdleState>(state => !_playerController.IsAttacking, (int)EntityStateMachineLayer.STATE);

        MakeAnyTransition<AttackState>(state => _playerController.IsAttacking, (int)EntityStateMachineLayer.STATE);
    }
}
