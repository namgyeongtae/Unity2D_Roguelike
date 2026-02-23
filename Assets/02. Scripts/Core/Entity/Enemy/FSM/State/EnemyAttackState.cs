using UnityEngine;

public class EnemyAttackState : State<Entity>
{
    private EnemyAI enemyAI;
    private float remainingCoolTime = 0f;
    private float attackCoolTime = 0f;

    protected override void Setup()
    {
        enemyAI = Entity.GetComponent<EnemyAI>();

        attackCoolTime = Entity.Stats.GetStat(StatId.ATTACK_COOL_TIME).Value;
    }

    public override void Enter()
    {
        remainingCoolTime = 0f;
    }

    public override void Update()
    {
        remainingCoolTime -= Time.deltaTime;

        if (remainingCoolTime <= 0f)
        {   
            enemyAI.Attack(enemyAI.Target, Entity.Stats.GetStat(StatId.DAMAGE).Value);
            remainingCoolTime = attackCoolTime;
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit Attack");

        // remainingCoolTime = 0f;
    }
}
