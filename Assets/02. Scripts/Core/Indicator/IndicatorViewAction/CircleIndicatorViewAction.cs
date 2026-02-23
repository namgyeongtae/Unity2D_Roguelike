using UnityEngine;

[System.Serializable]
public class CircleIndicatorViewAction : IndicatorViewAction
{
    [SerializeField] private GameObject indicatorPrefab;

    [SerializeField] private float indicatorRadiusOverride;
    [SerializeField] private float indicaotrAngleOverride;

    [SerializeField] private bool isUseIndicatorFillAmount;

    [SerializeField] private bool isAttachIndicatorToRequester;

    private Indicator spawnedRangeIndicator;

    public override void ShowIndicator(TargetSearcher targetSearcher, GameObject requesterObject,
        object range, float angle, float fillAmount)
    {
        Debug.Assert(range is float, "CircleIndicatorViewAction::ShowIndicator - range는 float 타입이어야 합니다.");

        HideIndicator();

        fillAmount = isUseIndicatorFillAmount ? fillAmount :0f;

        var attachTarget = isAttachIndicatorToRequester ? requesterObject.transform : null;

        float radius = Mathf.Approximately(indicatorRadiusOverride, 0f) ? (float)range : indicatorRadiusOverride;

        angle = Mathf.Approximately(indicaotrAngleOverride, 0f) ? angle : indicaotrAngleOverride;

        spawnedRangeIndicator = GameObject.Instantiate(indicatorPrefab).GetComponent<Indicator>();
        spawnedRangeIndicator.Setup(angle, radius, fillAmount, attachTarget);
    }

    public override void HideIndicator()
    {
        if (!spawnedRangeIndicator)
            return;
        
        GameObject.Destroy(spawnedRangeIndicator.gameObject);
    }

    public override void SetFillAmount(float fillAmount)
    {
        if (!isUseIndicatorFillAmount || spawnedRangeIndicator == null)
            return;
        
        spawnedRangeIndicator.FillAmount = fillAmount;
    }

    public override object Clone()
    {
        return new CircleIndicatorViewAction
        {
            indicatorPrefab = indicatorPrefab,
            indicatorRadiusOverride = indicatorRadiusOverride,
            indicaotrAngleOverride = indicaotrAngleOverride,
            isUseIndicatorFillAmount = isUseIndicatorFillAmount,
            isAttachIndicatorToRequester = isAttachIndicatorToRequester,
        };
    }
}
