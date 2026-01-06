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
    public bool CanAttack => !isAttacking && !isHit;

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
        equippedWeapon.Setup();
        equippedWeapon.Equip(entity);

        entity.SocketPivot.gameObject.SetActive(false);

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

        if (!CanMove)
            return;

        entity.Movement?.Move(dir);
    }

    void Attack()
    {
        if (!CanAttack)
            return;

        isAttacking = true;
        
        entity.SocketPivot.gameObject.SetActive(true);

        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition = new Vector2(worldPosition.x, worldPosition.y);
        Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);

        Vector2 attackDirection = (mousePosition - playerPosition).normalized;
        equippedWeapon.Attack(attackDirection);

        if (attackDirection.y > 0) 
        {
            entity.Animator.Play("UpRightSlash");
            Direction = EntityDirection.Up;
        }
        else if (attackDirection.y < 0) 
        {
            entity.Animator.Play("DownRightSlash");
            Direction = EntityDirection.Down;
        }
    }

    public void AttackEnd()
    {
        entity.SocketPivot.gameObject.SetActive(false);

        isAttacking = false;
    }

    private void SetMoveAnimation(EntityDirection dir, EntityState state)
    {
        if (!CanMove)
            return;

        if (state != EntityState.Idle && state != EntityState.Walk)
            return;

        string clipName = $"{state}_{dir}";

        if (!entity.Animator.HasState(0, Animator.StringToHash(clipName)))
        {
            Debug.LogWarning($"Animation clip not found: {clipName}");
            return;
        }

        entity.Animator.Play(clipName);
    }
}
