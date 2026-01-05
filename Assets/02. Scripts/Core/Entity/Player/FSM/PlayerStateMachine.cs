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
        AddState<IdleState>((int)EntityStateMachineLayer.STATE);
        AddState<WalkState>((int)EntityStateMachineLayer.STATE);
        AddState<DashState>((int)EntityStateMachineLayer.STATE);
        AddState<DeadState>((int)EntityStateMachineLayer.STATE);
        AddState<AttackState>((int)EntityStateMachineLayer.STATE);

        AddState<UpState>((int)EntityStateMachineLayer.DIR);
        AddState<DownState>((int)EntityStateMachineLayer.DIR);
        AddState<LeftState>((int)EntityStateMachineLayer.DIR);
        AddState<RightState>((int)EntityStateMachineLayer.DIR);
    }

    protected override void MakeTransitions()
    {
        MakeAnyTransition<UpState>(state => Owner.Movement.MoveDir == Vector2.up, (int)EntityStateMachineLayer.DIR);
        MakeAnyTransition<DownState>(state => Owner.Movement.MoveDir == Vector2.down, (int)EntityStateMachineLayer.DIR);
        MakeAnyTransition<LeftState>(state => Owner.Movement.MoveDir == Vector2.left, (int)EntityStateMachineLayer.DIR);
        MakeAnyTransition<RightState>(state => Owner.Movement.MoveDir == Vector2.right, (int)EntityStateMachineLayer.DIR);


        MakeTransition<IdleState, WalkState>(state => Owner.Movement.MoveDir != Vector2.zero, (int)EntityStateMachineLayer.STATE);
        MakeTransition<WalkState, IdleState>(state => Owner.Movement.MoveDir == Vector2.zero, (int)EntityStateMachineLayer.STATE);
        MakeTransition<AttackState, IdleState>(state => !_playerController.IsAttacking, (int)EntityStateMachineLayer.STATE);
        
        MakeAnyTransition<AttackState>(state => _playerController.IsAttacking, (int)EntityStateMachineLayer.STATE);
    }
}
