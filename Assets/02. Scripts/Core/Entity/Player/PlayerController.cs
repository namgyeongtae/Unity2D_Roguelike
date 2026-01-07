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
            SetAnimation(Direction, value);
        }
    }

    void Start()
    {
        entity = GetComponent<Entity>();
     
        Debug.Log($"PlayerController Start: {name}");

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
        if (!CanMove)
            return;

        if (dir.x != 0)
            Direction = dir.x > 0 ? EntityDirection.Right : EntityDirection.Left;
        else if (dir.y != 0)
            Direction = dir.y > 0 ? EntityDirection.Up : EntityDirection.Down;

        entity.Movement?.Move(dir);
    }

    void Attack()
    {
        if (!CanAttack)
            return;
        
        entity.SocketPivot.gameObject.SetActive(true);

        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition = new Vector2(worldPosition.x, worldPosition.y);
        Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);

        Vector2 attackDirection = (mousePosition - playerPosition).normalized;
        equippedWeapon.Attack(attackDirection);

        if (attackDirection.y > 0) 
        {
            entity.Animator.Play("UpRightSlash");
        }
        else if (attackDirection.y < 0) 
        {
            entity.Animator.Play("DownRightSlash");
        }
        
        isAttacking = true;
    }

    public void AttackEnd(Vector2 direction)
    {
        entity.SocketPivot.gameObject.SetActive(false);

        isAttacking = false;

        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY)
        {
            Direction = direction.x > 0 ? EntityDirection.Right : EntityDirection.Left;
        }
        else
        {
            Direction = direction.y > 0 ? EntityDirection.Up : EntityDirection.Down;
        }
    }

    public void SetAnimation(EntityDirection dir, EntityState state)
    {
        if (entity == null)
            return;

        switch (state)
        {
            case EntityState.Idle:
            case EntityState.Walk:
            case EntityState.Dodge:
            {
                string clipName = $"{state}_{dir}";
                entity.PlayAnimation(clipName);
                break;
            }  
        }
    }
}
