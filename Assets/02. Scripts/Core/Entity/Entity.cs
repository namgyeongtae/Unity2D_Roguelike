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

    public EntityControlType ControlType => _controlType;
    public bool IsPlayer => _controlType == EntityControlType.Player;

    public Animator Animator { get; private set;}

    public Stats Stats { get; private set; }

    public bool IsDead => Stats.HPStat != null && Mathf.Approximately(Stats.HPStat.Value, 0f);

    public EntityMovement Movement { get; private set; }

    public MonoStateMachine<Entity> StateMachine { get; private set; }

    public event OnTakeDamageHandler onTakeDamage;
    public event OnDeadHandler onDead;


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

    public void TakeDamage(Entity instigator, object causer, float damage)
    {
        if (IsDead)
            return;

        float prevValue = Stats.HPStat.DefaultValue;
        Stats.HPStat.DefaultValue -= damage;

        onTakeDamage?.Invoke(this, instigator, causer, damage);

        if (Mathf.Approximately(Stats.HPStat.DefaultValue, 0f))
            OnDead();
    }

    private void OnDead()
    {
        if (Movement)
            Movement.enabled = false;

        onDead?.Invoke(this);
    }

    private void OnDisable()
    {
        Debug.LogWarning("Entity is Disabled");
    }
}
