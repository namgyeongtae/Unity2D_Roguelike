using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Entity entity;
    private Weapon equippedWeapon;

    private bool isAttacking = false;
    private bool isHit = false;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    [SerializeReference, SubclassSelector] 
    private DamageAction[] _damageActions;
    
    private EntityDirection direction = EntityDirection.Down;
    public bool IsAttacking => isAttacking;

    public bool CanMove => !isAttacking && !isHit && !(entity.Movement as PlayerMovement).IsDodging;
    public bool CanAttack => !isAttacking && !isHit && !(entity.Movement as PlayerMovement).IsDodging;
    public bool CanDodge => entity.IsInState<WalkState>();

    public Entity Entity => entity;

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

    private void Awake()
    {
        entity = GetComponent<Entity>();

        equippedWeapon = GetComponentInChildren<Weapon>();
    }

    void Start()
    {
        equippedWeapon.Setup();
        equippedWeapon.Equip(entity);

        entity.SocketPivot.gameObject.SetActive(false);

        InstallKeyBindings();
    }

    void OnEnable()
    {
        KeyInputController.Instance.onDirectionInput += Move;

        entity.onTakeDamage += OnTakeDamage;
    }

    void OnDisable()
    {
        KeyInputController.Instance.onDirectionInput -= Move;

        entity.onTakeDamage -= OnTakeDamage;
    }

    void InstallKeyBindings()
    {
        KeyInputController.Instance.SetCommand(ActionId.Attack, InputPhase.Down, Attack);
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

    void Dodge()
    {
        
        var movement = entity.Movement as PlayerMovement;
        if (movement == null)
            return;
        
        Vector2 direction = movement.MoveDir;

        movement.Dodge(direction);
    }
    
    private void OnTakeDamage(Entity entity, Entity instigator, object causer, float damage)
    {
        foreach (var damageAction in _damageActions)
        {
            StartCoroutine(damageAction.OnDamage(entity, instigator, causer, damage));
        }
    }
}
