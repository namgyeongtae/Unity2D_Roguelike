using System;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    protected Entity entity;

    [SerializeField] protected SpriteRenderer _spriteRenderer;

    [SerializeReference, SubclassSelector] 
    protected DamageAction[] _damageActions;

    public Entity Entity => entity;

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
    }

    protected virtual void OnEnable()
    {
        entity.onTakeDamage += OnTakeDamage;
    }

    protected virtual void OnDisable()
    {
        entity.onTakeDamage -= OnTakeDamage;
    }

    protected virtual void OnTakeDamage(Entity entity, Entity instigator, object causer, float damage)
    {
        foreach (var damageAction in _damageActions)
        {
            StartCoroutine(damageAction.OnDamage(entity, instigator, causer, damage));
        }
    }
}
