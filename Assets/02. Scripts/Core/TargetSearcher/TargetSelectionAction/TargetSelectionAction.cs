using UnityEngine;

public abstract class TargetSelectionAction
{
    public delegate void SelectCompletedHandler(TargetSelectionResult result);

    [Header("Indicator")]
    [SerializeField]
    private bool isShowIndicatorPlayerOnly;
    [SerializeReference, SubclassSelector]
    private IndicatorViewAction indicatorViewAction;

    // Range에 Scale을 적용할지 여부
    [Header("Option")]
    [SerializeField]
    private bool isUseScale;

    private float scale;

    public float Scale
    {
        get => scale;
        set
        {
            if (scale == value)
                return;
            
            scale = value;
            indicatorViewAction?.SetFillAmount(scale);

            OnScaleChanged(scale);
        }
    }

    public abstract object Range { get; }
    public abstract object ScaledRange { get; }
    public abstract float Angle { get; }

    public object ProperRange => isUseScale ? ScaledRange : Range;

    public bool IsUseScale => isUseScale;

    public TargetSelectionAction() { }
    public TargetSelectionAction(TargetSelectionAction copy) 
    {
        indicatorViewAction = copy.indicatorViewAction?.Clone() as IndicatorViewAction;
        isUseScale = copy.isUseScale;
    }

    protected abstract TargetSelectionResult SelectImmediateByPlayer(TargetSearcher targetSearcher, Entity requesterEntity, 
        GameObject requesterObject, Vector3 position);

    protected abstract TargetSelectionResult SelectImmediateByAI(TargetSearcher targetSearcher, Entity requesterEntity, 
        GameObject requesterObject, Vector3 position);

    public TargetSelectionResult SelectImmediate(TargetSearcher targetSearcher, Entity requesterEntity, 
        GameObject requesterObject, Vector3 position)
        => requesterEntity.IsPlayer ? SelectImmediateByPlayer(targetSearcher, requesterEntity, requesterObject, position) 
                                    : SelectImmediateByAI(targetSearcher, requesterEntity, requesterObject, position);
    
    public abstract void Select(TargetSearcher targetSearcher, Entity requesterEntity, 
        GameObject requesterObject, SelectCompletedHandler onSelectCompleted);

    public abstract void CancelSelect(TargetSearcher targetSearcher);

    public abstract bool IsInRange(TargetSearcher targetSearcher, Entity requesterEntity, GameObject requesterObject, Vector3 targetPosition);

    public abstract object Clone();

    public virtual void ShowIndicator(TargetSearcher targetSearcher, GameObject requesterObject, float fillAmount)
    {
        var entity = requesterObject.GetComponent<Entity>();
        if (isShowIndicatorPlayerOnly && (entity == null || !entity.IsPlayer))
            return;
        
        indicatorViewAction?.ShowIndicator(targetSearcher, requesterObject, ProperRange, Angle, fillAmount);
    }

    public virtual void HideIndicator() => indicatorViewAction?.HideIndicator();

    protected virtual void OnScaleChanged(float newScale) { }
}
