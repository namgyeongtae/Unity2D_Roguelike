using UnityEngine;

[System.Serializable]
public class SelectSelfByOneClick : SelectTarget
{
    public SelectSelfByOneClick() { }

    public SelectSelfByOneClick(SelectSelfByOneClick copy) : base(copy) { }

    public override object Clone() => new SelectSelfByOneClick(this);

    protected override TargetSelectionResult SelectImmediateByPlayer(Vector2 screenPosition, TargetSearcher targetSearcher, 
        Entity requesterEntity, GameObject requesterObject)
        => new TargetSelectionResult(requesterObject, SearchResultMessage.FindTarget);

    protected override TargetSelectionResult SelectImmediateByAI(TargetSearcher targetSearcher, Entity requesterEntity, 
        GameObject requesterObject, Vector3 position)
        => SelectImmediateByPlayer(targetSearcher, requesterEntity, requesterObject, position);
}
