using UnityEngine;

public class EnemyAttackState : State<Entity>
{
    private float coolTime = 0f;

    protected override void Setup()
    {
        
    }

    public override void Enter()
    {
        Debug.Log("Enter Attack");

        coolTime = Entity.Stats.GetStat(StatId.ATTACK_COOL_TIME).Value;
    }

    public override void Update()
    {
        coolTime += Time.deltaTime;

        if (coolTime >= Entity.Stats.GetStat(StatId.ATTACK_COOL_TIME).Value)
        {
            Debug.Log("Attack");
            Entity.PlayAnimation("Attack", true, 0, 0f);
            coolTime = 0f;
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit Attack");

        coolTime = 0f;
    }
}
