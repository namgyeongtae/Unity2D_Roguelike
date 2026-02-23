using System.Collections.Generic;
using System.Linq;
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
    public delegate void OnHpChangedHandler(Entity entity, float hp, float maxHp);

    #endregion

    [SerializeField] private Category[] categories;

    [SerializeField] private EntityControlType _controlType;
    [SerializeField] private GameObject socketPivot;

    private Dictionary<string, Transform> socketsByName = new();

    public EntityControlType ControlType => _controlType;
    public bool IsPlayer => _controlType == EntityControlType.Player;

    public Animator Animator { get; private set;}

    public Stats Stats { get; private set; }

    public bool IsDead => Stats.HPStat != null && Mathf.Approximately(Stats.HPStat.Value, 0f);

    public EntityMovement Movement { get; private set; }

    public MonoStateMachine<Entity> StateMachine { get; private set; }
    public GameObject SocketPivot => socketPivot;
    public Category[] Categories => categories;

    public Entity Target { get; set; }

    public event OnTakeDamageHandler onTakeDamage;
    public event OnDeadHandler onDead;
    public event OnHpChangedHandler onHpChanged;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        
        Stats = GetComponent<Stats>();
        Stats.Setup(this);

        Movement = GetComponent<EntityMovement>();
        Movement.Setup(this);

        StateMachine = GetComponent<MonoStateMachine<Entity>>();
        StateMachine.Setup(this);

        if (Stats.HPStat != null)
            Stats.HPStat.onValueChanged += (stat, value, prevValue) => { onHpChanged?.Invoke(this, value, prevValue); };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(Entity instigator, object causer, float damage)
    {
        if (IsDead)
            return;

        Debug.Log($"TakeDamage: {damage}");

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

    private Transform GetTransformSocket(Transform root, string socketName)
    {
        if (root.name == socketName)
            return root;

        foreach (Transform child in root)
        {
            var socket = GetTransformSocket(child, socketName);
            if (socket)
                return socket;
        }

        return null;
    }

    public Transform GetTransformSocket(string socketName)
    {
        if (socketsByName.TryGetValue(socketName, out var socket))
            return socket;

        socket = GetTransformSocket(transform, socketName);

        if (socket)
            socketsByName[socketName] = socket;

        return socket;
    }

    public bool HasCategory(Category category)
        => categories.Any(x => x.ID == category.ID);

    public bool HasCategory(Category[] categories)
    {
        foreach (var category in categories)
        {
            if (HasCategory(category))
                return true;
        }
        return false;
    }

    public bool IsInState<T>() where T : State<Entity>
        => StateMachine.IsInState<T>();
    
    public bool IsInState<T>(int layer) where T : State<Entity>
        => StateMachine.IsInState<T>(layer);

    public void PlayAnimation(string clipName, bool needNormalize = false, int layer = 0, float normalizedTime = 0f)
    {
        if (!Animator.HasState(0, Animator.StringToHash(clipName)))
        {
            Debug.LogWarning($"Animation clip not found: {clipName}");
            return;
        }

        if (needNormalize)
            Animator.Play(clipName, layer, normalizedTime);
        else
            Animator.Play(clipName, layer);
    }
    
}
