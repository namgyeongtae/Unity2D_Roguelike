using UnityEngine;

public class EnemyDeadState : State<Entity>
{
    protected override void Setup()
    {
        
    }

    public override void Enter()
    {
        var enemyAI = Entity.GetComponent<EnemyAI>();
        if (enemyAI)
            enemyAI.enabled = false;

        Entity.enabled = false;

        Entity.PlayAnimation("Dead");
        
        GameObject.Destroy(Entity.gameObject, 2f);
    }
}
