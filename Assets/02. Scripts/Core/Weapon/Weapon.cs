using UnityEngine;

public class Weapon : MonoBehaviour
{
    public delegate void OnEquippedHandler(Entity entity);
    public delegate void OnAttackHandler(Vector2 direction);

    protected Entity owner;
    protected Animator animator;

    [SerializeField] protected Stat damageData;

    public event OnEquippedHandler onEquipped;
    public event OnAttackHandler onAttack;

    public virtual void Setup()
    {
        Debug.Log($"Weapon Start: {name}");

        animator = GetComponent<Animator>();
    }

    public virtual void Equip(Entity entity)
    {
        owner = entity;

        onEquipped?.Invoke(owner);
    }

    public virtual void Attack(Vector2 direction) 
    {
        onAttack?.Invoke(direction);
    }
}
