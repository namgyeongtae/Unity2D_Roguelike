using UnityEngine;

public class EnemyTraceState : State<Entity>
{
    private EnemyAI enemyAI;

    protected override void Setup()
    {
        enemyAI = Entity.GetComponent<EnemyAI>();
    }

    public override void Enter()
    {
        Debug.Log("Enter Trace");
    }

    public override void Update()
    {
        if (enemyAI.Target == null)
        {
            Entity.StateMachine.ExecuteCommand(EnemyStateTransitionCommand.TO_IDLE_STATE, 0);
            return;
        }

        Vector2 dir = enemyAI.Target.transform.position - Entity.transform.position;

        Entity.transform.localScale = new Vector3(dir.x < 0 ? -1 : 1, 1, 1);

        Entity.Movement.Move(dir.normalized);
    }

    public override void Exit()
    {
        Debug.Log("Exit Trace");
    }
}
