using UnityEngine;

public enum EnemyStateTransitionCommand
{
    TO_IDLE_STATE = 0,
    TO_PATROL_STATE = 1,
    TO_TRACE_STATE = 2,
    TO_ATTACK_STATE = 3,
}

public class EnemyStateMachine : MonoStateMachine<Entity>
{
    private EnemyAI enemyAI;

    void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
    }

    protected override void AddStates()
    {
        AddState<EnemyIdleState>((int)EntityStateMachineLayer.STATE);
        AddState<EnemyPatrolState>((int)EntityStateMachineLayer.STATE);
        AddState<EnemyTraceState>((int)EntityStateMachineLayer.STATE);
        AddState<EnemyAttackState>((int)EntityStateMachineLayer.STATE);
    }

    protected override void MakeTransitions()
    {
        // Idle -> Patrol   : Target이 null이고 랜덤 Time 마다 Patrol로 전환
        MakeTransition<EnemyIdleState, EnemyPatrolState>(EnemyStateTransitionCommand.TO_PATROL_STATE, state => enemyAI.Target == null, 0);
        
        // Idle -> Trace    : Target이 null이 아니고 DETECT_DIST 이내에 있으면 전환
        MakeTransition<EnemyIdleState, EnemyTraceState>(state => enemyAI.IsTargetInDetectRange(), 0);

        // Patrol -> Idle   : Target이 null이고, Patrol 목표지점 도착 시 랜덤결과로 Idle로 전환 (바로 Patrol이 다시 잡힐 수 있음) -> canTransitionToSelf = true
        MakeTransition<EnemyPatrolState, EnemyIdleState>(EnemyStateTransitionCommand.TO_IDLE_STATE, state => enemyAI.Target == null, 0);
        
        // Patrol -> Patrol : Target이 null이고, Patrol 목표지점 도착 시 랜덤결과로 Patrol로 전환
        MakeTransition<EnemyPatrolState, EnemyPatrolState>(EnemyStateTransitionCommand.TO_PATROL_STATE, state => enemyAI.Target == null, 0);

        // Patrol -> Trace  : Target이 DETECT_DIST 이내에 있으면 전환
        MakeTransition<EnemyPatrolState, EnemyTraceState>(state => enemyAI.IsTargetInDetectRange(), 0);

        // Trace -> Idle    : Target이 DETECT_DIST 밖에 있으면 전환 | target이 null이 되면
        MakeTransition<EnemyTraceState, EnemyIdleState>(EnemyStateTransitionCommand.TO_IDLE_STATE, state => enemyAI.Target == null, 0);

        // Trace -> Attack  : Target이 ATTACK_DIST 이내에 있으면 전환
        MakeTransition<EnemyTraceState, EnemyAttackState>(state => enemyAI.IsTargetInAttackRange(), 0);
        
        // Attack -> Trace  : Target이 ATTACK_DIST 밖에 있고 DETECT_DIST 이내에 있으면 전환
        MakeTransition<EnemyAttackState, EnemyTraceState>(state => enemyAI.IsTargetInDetectRange() && !enemyAI.IsTargetInAttackRange(), 0);

        // Attack -> Idle   : target이 null이 되면
        MakeTransition<EnemyAttackState, EnemyIdleState>(state => enemyAI.Target == null, 0);
    }
}
