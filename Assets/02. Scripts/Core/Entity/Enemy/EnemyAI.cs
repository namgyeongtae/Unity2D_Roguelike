using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public delegate void OnTargetChangedHandler();

    private Entity entity;
    private GameObject target;

    public Entity Entity => entity;
    public GameObject Target => target;

    public float DetectDistance => entity.Stats.GetStat(StatId.DETECT_DIST).Value;
    public float AttackDistance => entity.Stats.GetStat(StatId.ATTACK_DIST).Value;

    public event OnTargetChangedHandler onTargetChanged;

    void Awake()
    {
        entity = GetComponent<Entity>();
    }

    void Update()
    {

    }

    public void DetectTarget()
    {
        if (target != null)
            return;

        float detectDistance = entity.Stats.GetStat(StatId.DETECT_DIST).Value;

        var targetCol = Physics2D.OverlapCircle(transform.position, detectDistance, LayerMask.GetMask("Player"));
        
        if (targetCol != null)
            SetTarget(targetCol.gameObject);
    }

    private void SetTarget(GameObject targetObject)
    {
        target = targetObject;

        onTargetChanged?.Invoke();
    }

    public bool IsTargetInDetectRange()
    {
        if (target == null)
        {
            Debug.LogError("Target is null");
            return false;
        }

        return Vector2.Distance(transform.position, target.transform.position) <= DetectDistance;
    }

    public bool IsTargetInAttackRange()
    {
        if (target == null)
        {
            Debug.LogError("Target is null");
            return false;
        }

        return Vector2.Distance(transform.position, target.transform.position) <= AttackDistance;
    }
}
