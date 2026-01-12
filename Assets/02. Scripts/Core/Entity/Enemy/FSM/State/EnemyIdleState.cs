using UnityEngine;

public class EnemyIdleState : State<Entity>
{
    private float idleRandTime = 0f;
    private float elapsedTime = 0f;

    public override void Enter()
    {
        idleRandTime = Random.Range(1.5f, 3f);
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= idleRandTime)
        {
            Debug.Log("Change to Patrol");
            Entity.StateMachine.ExecuteCommand(EnemyStateTransitionCommand.TO_PATROL_STATE, 0);
        }
    }

    public override void Exit()
    {
        idleRandTime = elapsedTime = 0f;
    }
}
