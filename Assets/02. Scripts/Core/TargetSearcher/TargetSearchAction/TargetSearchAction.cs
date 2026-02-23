using System;
using UnityEngine;

[System.Serializable]
public abstract class TargetSearchAction : ICloneable
{
    [Header("Indicator")]
    [SerializeField]
    private bool isShowIndicatorPlayerOnly;

    [SerializeReference, SubclassSelector]
    private IndicatorViewAction indicatorViewAction;

    [Header("Option")]
    [SerializeField]
    private bool isUseScale;

    // Range에 적용되어 Range 값을 조절할 때 사용되는 변수
    // Skill의 Charge 정도에 따라 검색 범위가 달라지게 할 때 활용할 수 있음음
    private float scale;

    public float Scale
    {
        get => scale;
        set
        {
            if (scale == value)
                return;
            
            scale = value;

            // scale의 Indicator의 FillAmount로 적용
            indicatorViewAction?.SetFillAmount(scale);

            OnScaleChanged(scale);
        }
    }

    public abstract object Range { get; }
    public abstract object ScaledRange { get; }
    public abstract float Angle { get; }
    public object ProperRange => isUseScale ? ScaledRange : Range;

    public bool IsUseScale => isUseScale;

    public TargetSearchAction() { }
    public TargetSearchAction(TargetSearchAction copy)
    {
        indicatorViewAction = copy.indicatorViewAction?.Clone() as IndicatorViewAction;
        isUseScale = copy.isUseScale;
    }

    public abstract TargetSearchResult Search(TargetSearcher targetSearcher, Entity requesterEntity, 
        GameObject requesterObject, TargetSelectionResult selectResult);
    
    public abstract object Clone();

    public virtual void ShowIndicator(TargetSearcher targetSearcher, GameObject requesterObject, float fillAmount)
    {
        var entity = requesterObject.GetComponent<Entity>();
        if (isShowIndicatorPlayerOnly && (entity == null || !entity.IsPlayer))
            return;
        
        indicatorViewAction?.ShowIndicator(targetSearcher, requesterObject, Range, Angle, fillAmount);
    }

    public virtual void HideIndicator() => indicatorViewAction?.HideIndicator();

    public virtual void OnScaleChanged(float newScale) { }
}
