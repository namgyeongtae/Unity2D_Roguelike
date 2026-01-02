using UnityEngine;

public enum EntityControlType
{
    Player,
    AI
}

public class Entity : MonoBehaviour
{
    #region Delegate
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity">데미지를 받는 Entity</param>
    /// <param name="instigator">데미지를 가한 Entity</param>
    /// <param name="causer">데미지를 가한 직접적인 주체 (ex. 총알, 폭발물 etc...) -> instigator가 causer를 날리는 상황</param>
    /// <param name="damage">데미지 값</param>
    public delegate void OnTakeDamageHandler(Entity entity, Entity instigator, object causer, float damage);
    
    public delegate void OnDeadHandler(Entity entity);

    #endregion


    [SerializeField] private EntityControlType _controlType;

    private EntityDirection _direction = EntityDirection.Down;
    private EntityState _state = EntityState.Idle;

    public EntityControlType ControlType => _controlType;
    public bool IsPlayer => _controlType == EntityControlType.Player;

    public Animator Animator { get; private set;}

    public Stats Stats { get; private set; }

    public bool IsDead => Stats.HPStat != null && Mathf.Approximately(Stats.HPStat.Value, 0f);

    public EntityMovement Movement { get; private set; }

    public MonoStateMachine<Entity> StateMachine { get; private set; }

    public EntityDirection Direction
    {
        get => _direction;
        set
        {
            if (_direction == value)
                return;

            _direction = value;
            SetAnimation(value, _state);
        }
    }
    public EntityState State
    {
        get => _state;
        set
        {
            if (_state == value)
                return;

            _state = value;
            SetAnimation(Direction, value);
        }
    }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        
        Stats = GetComponent<Stats>();
        Stats.Setup(this);

        Movement = GetComponent<EntityMovement>();
        Movement.Setup(this);

        StateMachine = GetComponent<MonoStateMachine<Entity>>();
        StateMachine.Setup(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetAnimation(EntityDirection dir, EntityState state)
    {
        string direction = dir switch
        {
            EntityDirection.Left => "Side",
            EntityDirection.Right => "Side",
            EntityDirection.Up => "Up",
            EntityDirection.Down => "Down",
            _ => throw new System.Exception("Invalid direction")
        };
        
        string clipName = $"{state}_{direction}";

        if (!Animator.HasState(0, Animator.StringToHash(clipName)))
        {
            Debug.LogError($"Animation clip not found: {clipName}");
            return;
        }

        Animator.Play(clipName);
    }
}
