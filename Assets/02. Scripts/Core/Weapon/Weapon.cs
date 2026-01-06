using UnityEngine;

public class Weapon : MonoBehaviour
{
    public delegate void OnEquippedHandler(Entity entity);

    protected Entity owner;
    protected Animator animator;

    [SerializeField] protected Stat damageData;

    public event OnEquippedHandler onEquipped;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void Equip(Entity entity)
    {
        owner = entity;

        onEquipped?.Invoke(owner);
    }

    public virtual void Attack(Vector2 direction) { }
}
