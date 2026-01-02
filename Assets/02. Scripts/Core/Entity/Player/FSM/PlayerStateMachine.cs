using UnityEngine;

public enum EntityStateMachineLayer
{
    DIR = 0,
    STATE = 1,

}

public class PlayerStateMachine : MonoStateMachine<Entity>
{
    protected override void AddStates()
    {
        AddState<IdleState>((int)EntityStateMachineLayer.STATE);
        AddState<WalkState>((int)EntityStateMachineLayer.STATE);
        AddState<DashState>((int)EntityStateMachineLayer.STATE);
        AddState<DeadState>((int)EntityStateMachineLayer.STATE);

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

        /* MakeTransition<IdleState, WalkState>(EntityState.Idle, EntityState.Walk, (int)EntityStateMachineLayer.STATE);
        MakeTransition<WalkState, IdleState>(EntityState.Walk, EntityState.Idle, (int)EntityStateMachineLayer.STATE);
        MakeTransition<WalkState, DashState>(EntityState.Walk, EntityState.Dash, (int)EntityStateMachineLayer.STATE);
        MakeTransition<DashState, IdleState>(EntityState.Dash, EntityState.Idle, (int)EntityStateMachineLayer.STATE);
        MakeTransition<DashState, DeadState>(EntityState.Dash, EntityState.Dead, (int)EntityStateMachineLayer.STATE); */
    }
}
