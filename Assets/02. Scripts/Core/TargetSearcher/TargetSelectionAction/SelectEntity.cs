using System.Linq;
using UnityEngine;

[System.Serializable]
public class SelectEntity : SelectTarget
{
    [SerializeField] private bool isIncludeSelf;

    [SerializeField] private bool isSelectSameCategory;

    public SelectEntity() { }

    public SelectEntity(SelectEntity copy)
        : base(copy)
    {
        isIncludeSelf = copy.isIncludeSelf;
        isSelectSameCategory = copy.isSelectSameCategory;
    }

    protected override TargetSelectionResult SelectImmediateByPlayer(Vector2 screenPosition, TargetSearcher targetSearcher, 
        Entity requesterEntity, GameObject requesterObject)
    {
        var ray = Camera.main.ScreenPointToRay(screenPosition);
        
        RaycastHit2D hitInfo = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

        if (hitInfo)
        {
            var entity = hitInfo.collider.GetComponent<Entity>();

            if (entity == null || entity.IsDead || (entity == requesterEntity && !isIncludeSelf))
                return new TargetSelectionResult(hitInfo.point, SearchResultMessage.Fail);

            if (entity != requesterEntity)
            {
                var hasCategory = requesterEntity.Categories.Any(x => entity.HasCategory(x));

                if ((hasCategory && !isSelectSameCategory) || (!hasCategory && isSelectSameCategory))
                    return new TargetSelectionResult(hitInfo.point, SearchResultMessage.Fail);
            }

            if (IsInRange(targetSearcher, requesterEntity, requesterObject, hitInfo.point))
                return new TargetSelectionResult(entity.gameObject, SearchResultMessage.FindTarget);
            else
                return new TargetSelectionResult(entity.gameObject, SearchResultMessage.OutOfRange);
        }
        else
            return new TargetSelectionResult(requesterObject.transform.position, SearchResultMessage.Fail);
    }

    protected override TargetSelectionResult SelectImmediateByAI(TargetSearcher targetSearcher, Entity requesterEntity, 
        GameObject requesterObject, Vector3 position)
    {
        var target = requesterEntity.Target;

        if (!target)
            return new TargetSelectionResult(position, SearchResultMessage.Fail);
        else if (targetSearcher.IsInRange(requesterEntity, requesterObject, target.transform.position))
            return new TargetSelectionResult(target.gameObject, SearchResultMessage.FindTarget);
        else
            return new TargetSelectionResult(target.gameObject, SearchResultMessage.OutOfRange);
    }

    public override object Clone() => new SelectEntity(this);
}
