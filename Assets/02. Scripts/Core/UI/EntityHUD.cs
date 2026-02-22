using UnityEngine;
using UnityEngine.UI;

public class EntityHUD : MonoBehaviour
{
    [SerializeField] private Slider hpBar;

    private Entity entity;

    void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    void OnEnable()
    {
        entity.onTakeDamage += SetHP;
    }

    void OnDisable()
    {
        entity.onTakeDamage -= SetHP;
    }

    public void SetHP(Entity entity, Entity instigator, object causer, float damage)
    {
        Debug.Log($"SetHP: {entity.Stats.HPStat.DefaultValue} / {entity.Stats.MaxHpStat.DefaultValue}");
        hpBar.value = entity.Stats.HPStat.DefaultValue / entity.Stats.MaxHpStat.DefaultValue;
    }
}
