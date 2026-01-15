using UnityEngine;

public class EnemyPatrolState : State<Entity>
{
    private Vector3 originPosition;
    private float randTime = 0f;
    private float elapsedTime = 0f;
    private Vector2 patrolDirection = Vector2.zero;

    private float patrolRadius = 2f;

    protected override void Setup()
    {
        originPosition = Entity.transform.position;
    }

    public override void Enter()
    {
        randTime = Random.Range(1.5f, 3f);
        patrolDirection = ChangePatrolDirection();

        Entity.transform.localScale = new Vector3(patrolDirection.x < 0 ? -1 : 1, 1, 1);
        

        Entity.PlayAnimation("Walk");
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime < randTime)
            Entity.Movement.Move(patrolDirection);
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
        randTime = elapsedTime = 0f;
        patrolDirection = Vector2.zero;
    }

    private Vector2 ChangePatrolDirection()
    {
        Vector2 dir = Vector2.zero;

        // originPosition과의 거리에 따라 
        // 너~무 멀다 싶으면 가까워지는 방향중 랜덤으로 방향 선정
        // 좀 가깝다 싶으면 멀어지는 방향중 랜덤으로 방향 선정
        Vector3 relativeDir = Entity.transform.position - originPosition;
        float distance = relativeDir.magnitude;

        if (distance < 0.001f)
        {
            dir = Random.insideUnitCircle.normalized;
            return dir;
        }

        Vector3 baseDir;
        float halfRadius = patrolRadius * 0.5f;
        if (distance > halfRadius)
        {
            // 너~무 멀다 싶으면 가까워지는 방향중 랜덤으로 방향 선정
            baseDir = -relativeDir.normalized;
        }
        else
        {
            // 좀 가깝다 싶으면 멀어지는 방향중 랜덤으로 방향 선정
            baseDir = relativeDir.normalized;
        }

        float maxAngle = 40f;
        float randAngle = Random.Range(-maxAngle, maxAngle);

        Vector3 rotatedDir = MathUtils.Rotate(baseDir, randAngle);

        dir = rotatedDir.normalized;

        return dir;
    }
}
