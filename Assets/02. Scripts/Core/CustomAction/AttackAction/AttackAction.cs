using UnityEngine;
using System;

public enum AttackApplyType
{
    Instant,
    Animation
}

[System.Serializable]
public abstract class AttackAction
{
    [SerializeField] private AttackApplyType _applyType;

    public AttackApplyType ApplyType => _applyType;

    public virtual void Apply(Entity entity, Entity target, float damage) { }
    public virtual void Attack(Entity entity, Entity target, float damage)
    {
        entity.PlayAnimation("Attack", true, 0, 0f);

        if (_applyType == AttackApplyType.Instant)
            Apply(entity, target, damage);
    }
}
