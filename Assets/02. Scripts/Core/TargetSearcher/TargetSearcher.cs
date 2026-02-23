using UnityEngine;

[System.Serializable]
public class TargetSearcher
{
    public delegate void SelectionCompletedHandler(TargetSearcher targetSearcher, TargetSelectionResult targetSelectionResult);

    [Header("Select Action")]
    [SerializeReference, SubclassSelector]
    private TargetSelectionAction selectionAction;

    [Header("Search Action")]
    [SerializeReference, SubclassSelector]
    private TargetSearchAction searchAction;

    private SelectionCompletedHandler onSelectionCompleted;

    private float scale = 1f;

    public float Scale
    {
        get => scale;
        set
        {
            scale = Mathf.Clamp01(value);
            selectionAction.Scale = scale;
            searchAction.Scale = scale;
        }
    }

    public object SelectionRange => selectionAction.Range;
    public object SelectionScaledRange => selectionAction.ScaledRange;
    public object SelectionProperRange => selectionAction.ProperRange;
    public float SelectionAngle => selectionAction.Angle;

    public object SearchRange => searchAction.Range;
    public object SearchScaledRange => searchAction.ScaledRange;
    public object SearchProperRange => searchAction.ProperRange;
    public float SearchAngle => searchAction.Angle;

    public bool IsSearching { get; private set; }

    public TargetSelectionResult SelectionResult { get; private set; }
    public TargetSearchResult SearchResult { get; private set; }

    public TargetSearcher() { }

    public TargetSearcher(TargetSearcher copy)
    {
        selectionAction = copy.selectionAction.Clone() as TargetSelectionAction;
        searchAction = copy.searchAction.Clone() as TargetSearchAction;
        Scale = copy.Scale;
    }

    public void SelectTarget(Entity requesterEntity, GameObject requesterObject, SelectionCompletedHandler onSelectionCompleted)
    {
        CancelSelect();

        IsSearching = true;
        this.onSelectionCompleted = onSelectionCompleted;

        selectionAction.Select(this, requesterEntity, requesterObject, OnSelectionCompleted);
    }

    public TargetSelectionResult SelectImmediate(Entity requesterEntity, GameObject requesterObject, Vector3 position)
    {
        CancelSelect();

        SelectionResult = selectionAction.SelectImmediate(this, requesterEntity, requesterObject, position);
        return SelectionResult;
    }



    public void CancelSelect()
    {
        if (!IsSearching)
            return;
        
        IsSearching = false;
        selectionAction.CancelSelect(this);
    }

    public TargetSearchResult SearchTargets(Entity requesterEntity, GameObject requesterObject)
    {
        SearchResult = searchAction.Search(this, requesterEntity, requesterObject, SelectionResult);
        return SearchResult;
    }

    public void ShowIndicator(GameObject requesterObject)
    {
        HideIndicator();

        selectionAction.ShowIndicator(this, requesterObject, Scale);
        searchAction.ShowIndicator(this, requesterObject, Scale);
    }

    public void HideIndicator()
    {
        selectionAction.HideIndicator();
        searchAction.HideIndicator();
    }

    public bool IsInRange(Entity requesterEntity, GameObject requesterObject, Vector3 targetPosition)
        => selectionAction.IsInRange(this, requesterEntity, requesterObject, targetPosition);

    public void OnSelectionCompleted(TargetSelectionResult selectResult)
    {
        IsSearching = false;
        SelectionResult = selectResult;
        onSelectionCompleted.Invoke(this, selectResult);
    }
}
