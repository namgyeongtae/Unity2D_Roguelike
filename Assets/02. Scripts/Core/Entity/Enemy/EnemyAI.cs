using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public delegate void OnTargetChangedHandler();

    private Entity entity;
    private Entity target;

    public Entity Entity => entity;
    public Entity Target => target;

    public float DetectDistance => entity.Stats.GetStat(StatId.DETECT_DIST).Value;
    public float AttackDistance => entity.Stats.GetStat(StatId.ATTACK_DIST).Value;

    public event OnTargetChangedHandler onTargetChanged;

    void Awake()
    {
        entity = GetComponent<Entity>();
    }

    void Update()
    {
        DetectTarget();
    }

    public void DetectTarget()
    {
        float detectDistance = entity.Stats.GetStat(StatId.DETECT_DIST).Value;

        var targetCol = Physics2D.OverlapCircle(transform.position, detectDistance, LayerMask.GetMask("Player"));

        if (targetCol == null)
        {
            SetTarget(null);
            Debug.Log("No target found");
            return;
        }
        
        var targetEntity = targetCol.gameObject.GetComponent<Entity>() ?? targetCol.gameObject.GetComponentInParent<Entity>();

        if (targetCol != null)
            SetTarget(targetEntity);
    }

    private void SetTarget(Entity targetObject)
    {
        target = targetObject;

        onTargetChanged?.Invoke();
    }

    public bool IsTargetInDetectRange()
    {
        if (target == null)
        {
            // Debug.LogError("Target is null");
            return false;
        }

        return Vector2.Distance(transform.position, target.transform.position) <= DetectDistance;
    }

    public bool IsTargetInAttackRange()
    {
        if (target == null)
        {
            // Debug.LogError("Target is null");
            return false;
        }

        return Vector2.Distance(transform.position, target.transform.position) <= AttackDistance;
    }
}
