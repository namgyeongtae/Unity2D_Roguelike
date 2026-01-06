using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Entity entity;
    private Weapon equippedWeapon;

    private bool isAttacking = false;
    private bool isHit = false;

    private EntityDirection direction = EntityDirection.Down;
    private EntityState state = EntityState.Idle;


    public bool IsAttacking => isAttacking;

    public bool CanMove => !isAttacking && !isHit;
    public bool CanAttack => !isHit;

    public EntityDirection Direction
    {
        get => direction;
        set
        {
            if (direction == value)
                return;

            direction = value;
            SetMoveAnimation(value, state);
        }
    }
    public EntityState State
    {
        get => state;
        set
        {
            if (state == value)
                return;

            state = value;
            SetMoveAnimation(Direction, value);
        }
    }

    void Start()
    {
        entity = GetComponent<Entity>();
        equippedWeapon = GetComponentInChildren<Weapon>();

        InstallKeyBindings();
    }

    void OnEnable()
    {
        KeyInputController.Instance.onDirectionInput += Move;
    }

    void OnDisable()
    {
        KeyInputController.Instance.onDirectionInput -= Move;
    }

    void InstallKeyBindings()
    {
        KeyInputController.Instance.SetCommand(ActionId.Attack, InputPhase.Down, Attack);
    }
    
    void Move(Vector2 dir)
    {
        Debug.Log("Move");

        //if (!CanMove)
        //    return;

        entity.Movement?.Move(dir);
    }

    void Attack()
    {
        isAttacking = true;

        Vector2 mousePosition = Input.mousePosition;
        Vector2 playerPosition = transform.position;
        Vector2 attackDirection = (mousePosition - playerPosition).normalized;

        equippedWeapon.Attack(attackDirection);
    }

    public void AttackEnd()
    {
        isAttacking = false;
    }

    private void SetMoveAnimation(EntityDirection dir, EntityState state)
    {
        if (state != EntityState.Idle && state != EntityState.Walk)
            return;

        string clipName = $"{state}_{dir}";

        Debug.Log($"entity.name: {entity.name}, Animator: {entity.Animator.name}");

        if (!entity.Animator.HasState(0, Animator.StringToHash(clipName)))
        {
            Debug.LogWarning($"Animation clip not found: {clipName}");
            return;
        }

        entity.Animator.Play(clipName);
    }
}
