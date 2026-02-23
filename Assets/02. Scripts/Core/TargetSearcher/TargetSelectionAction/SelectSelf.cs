using UnityEngine;

[System.Serializable]
public class SelectSelf : TargetSelectionAction
{
    public override object Range => 0f;
    public override object ScaledRange => 0f;
    public override float Angle => 0f;

    public SelectSelf() { }

    public SelectSelf(SelectSelf copy) : base(copy) { }

    protected override TargetSelectionResult SelectImmediateByPlayer(TargetSearcher targetSearcher, Entity requesterEntity, 
        GameObject requesterObject, Vector3 position)
        => new (requesterObject, SearchResultMessage.FindTarget);
    
    protected override TargetSelectionResult SelectImmediateByAI(TargetSearcher targetSearcher, Entity requesterEntity, 
        GameObject requesterObject, Vector3 position)
        => SelectImmediateByPlayer(targetSearcher, requesterEntity, requesterObject, position);

    public override void Select(TargetSearcher targetSearcher, Entity requesterEntity, 
        GameObject requesterObject, SelectCompletedHandler onSelectCompleted)
        => onSelectCompleted.Invoke(SelectImmediateByPlayer(targetSearcher, requesterEntity, requesterObject, Vector2.zero));

    public override void CancelSelect(TargetSearcher targetSearcher) { } // 지연 없이 바로 Select가 되므로 취소할 상황이 없음

    public override bool IsInRange(TargetSearcher targetSearcher, Entity requesterEntity, GameObject requesterObject, Vector3 targetPosition)
        => true;
    
    public override object Clone() => new SelectSelf(this);
}
