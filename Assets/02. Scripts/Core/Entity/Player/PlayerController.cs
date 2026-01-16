using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Entity entity;
    private Weapon equippedWeapon;

    private bool isAttacking = false;
    private bool isHit = false;
    
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

        entity.onTakeDamage += Knockback;
    }

    void OnDisable()
    {
        KeyInputController.Instance.onDirectionInput -= Move;
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

    private void Knockback(Entity entity, Entity instigator, object causer, float damage)
    {
        if (isHit)
            return;

        StartCoroutine(KnockbackCoroutine(instigator));
    }

    private IEnumerator KnockbackCoroutine(Entity instigator)
    {
        Debug.Log("Knockback");

        // 넉백 중 플래그 설정
        isHit = true;
        
        // 넉백 파라미터
        float knockbackDistance = 1f;  // 넉백 거리
        float knockbackDuration = 0.3f; // 넉백 지속 시간
        
        // instigator와의 방향 계산 (instigator에서 플레이어로의 방향)
        Vector3 directionToPlayer = (transform.position - instigator.transform.position).normalized;
        
        Debug.Log("instigatorPosition: " + instigator.transform.position);
        Debug.Log("playerPosition: " + transform.position);
        Debug.Log("directionToPlayer: " + directionToPlayer);

        // 목표 위치 계산
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + directionToPlayer * knockbackDistance;
        targetPosition.z = startPosition.z;

        Debug.Log("startPosition: " + startPosition);
        Debug.Log("targetPosition: " + targetPosition);
        
        // 넉백 애니메이션
        float elapsedTime = 0f;
        
        while (elapsedTime < knockbackDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / knockbackDuration;
            
            // 이징 함수 적용 (예: ease-out)
            float easedT = 1f - Mathf.Pow(1f - t, 3f);
            
            // 위치 보간
            transform.position = Vector3.Lerp(startPosition, targetPosition, easedT);
            
            yield return null;
        }
        
        // 최종 위치 보정
        transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        
        // 넉백 완료
        isHit = false;
    }
}
