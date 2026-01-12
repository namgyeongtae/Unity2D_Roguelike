using UnityEngine;

public class EnemyPatrolState : State<Entity>
{
    private float randTime = 0f;
    private float elapsedTime = 0f;
    private Vector2 randomDir = Vector2.zero;

    public override void Enter()
    {

        randTime = Random.Range(1.5f, 3f);
        randomDir = MathUtils.AngleToDir(Random.Range(0f, 360f));
        
        Debug.Log($"Enter Patrol : {randTime}, {randomDir}");
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;

        Debug.Log($"Update Patrol ::: {elapsedTime.ToString("0.##")} / {randTime.ToString("0.##")}");

        if (elapsedTime < randTime)
            Entity.Movement.Move(randomDir);
        else
        {
            // Change State Random (Idle or Patrol)
            if (Random.Range(0f, 1f) < 0.5f)
                Entity.StateMachine.ExecuteCommand(EnemyStateTransitionCommand.TO_IDLE_STATE, 0);
            else
                Entity.StateMachine.ExecuteCommand(EnemyStateTransitionCommand.TO_PATROL_STATE, 0);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit Patrol");

        randTime = elapsedTime = 0f;
        randomDir = Vector2.zero;
    }
}
