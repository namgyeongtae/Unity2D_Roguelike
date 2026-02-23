using UnityEngine;

[System.Serializable]
public class SelectedTarget : TargetSearchAction
{
    public override object Range => 0f;
    public override object ScaledRange => 0f;
    public override float Angle => 0f;

    public SelectedTarget() { }
    public SelectedTarget(SelectedTarget copy) : base(copy) { }

    public override object Clone() => new SelectedTarget();

    public override TargetSearchResult Search(TargetSearcher targetSearcher, Entity requesterEntity, 
        GameObject requesterObject, TargetSelectionResult selectResult)
    {
        return selectResult.selectedTarget ? new TargetSearchResult(new[] { selectResult.selectedTarget }) :
            new TargetSearchResult(new[] { selectResult.selectedPosition });
    }
}
