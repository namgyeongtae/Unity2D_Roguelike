using UnityEngine;

public class DamageGiver : MonoBehaviour, IDamageable
{
    private Entity entity;

    void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    public void GiveDamage(Entity damageTaker)
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var targetEntity = other.GetComponent<Entity>();
        if (!entity.HasCategory(targetEntity.Categories))
        {
            targetEntity.TakeDamage(entity, this, entity.Stats.GetStat(StatId.DAMAGE).Value);
        }
    }
}
