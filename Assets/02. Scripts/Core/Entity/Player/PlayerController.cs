using System.Collections;
using UnityEngine;

public class PlayerController : EntityController
{
    private Weapon equippedWeapon;

    private bool isAttacking = false;
    private bool isHit = false;
    
    private EntityDirection direction = EntityDirection.Down;
    public bool IsAttacking => isAttacking;

    public bool CanMove => !isAttacking && !isHit && !(entity.Movement as PlayerMovement).IsDodging;
    public bool CanAttack => !isAttacking && !isHit && !(entity.Movement as PlayerMovement).IsDodging;
    public bool CanDodge => entity.IsInState<WalkState>();

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

    protected override void Awake()
    {
        base.Awake();

        equippedWeapon = GetComponentInChildren<Weapon>();
    }

    void Start()
    {
        equippedWeapon.Setup();
        equippedWeapon.Equip(entity);

        entity.SocketPivot.gameObject.SetActive(false);

        InstallKeyBindings();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        KeyInputController.Instance.onDirectionInput += Move;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        KeyInputController.Instance.onDirectionInput -= Move;
    }

    void InstallKeyBindings()
    {
        // KeyInputController.Instance.SetCommand(ActionId.Attack, InputPhase.Down, Attack);
        MouseController.Instance.onLeftClick += Attack;
        KeyInputController.Instance.SetCommand(ActionId.Dodge, InputPhase.Down, Dodge);
    }
    
    public void Move(Vector2 dir)
    {
        if (dir == Vector2.zero)
        {
            StopMove();
            return;
        }

        if (!CanMove || (entity.Movement.MoveDir != Vector2.zero
            && entity.Movement.MoveDir != dir))
            return;
        
        if (dir.x != 0)
            Direction = dir.x > 0 ? EntityDirection.Right : EntityDirection.Left;
        else if (dir.y != 0)
            Direction = dir.y > 0 ? EntityDirection.Up : EntityDirection.Down;

        entity.Movement?.Move(dir);
    }

    public void StopMove()
    {
        entity.Movement?.Stop();
    }

    void Attack(Vector2 mousePosition)
    {
        if (!CanAttack)
            return;
        
        entity.SocketPivot.gameObject.SetActive(true);

        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 mouseWorldPosition = new Vector2(worldPosition.x, worldPosition.y);
        Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);

        Vector2 attackDirection = (mouseWorldPosition - playerPosition).normalized;
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

    void Dodge()
    {
        var movement = entity.Movement as PlayerMovement;
        if (movement == null)
            return;
        
        Vector2 direction = movement.MoveDir;

        movement.Dodge(direction);
    }
}
