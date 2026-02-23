using UnityEngine;

public enum SearchResultMessage
{
    Fail,
    OutOfRange,
    FindTarget,
    FindPosition
}

public readonly struct TargetSelectionResult
{
    public readonly GameObject selectedTarget;
    
    public readonly Vector3 selectedPosition;
    public readonly SearchResultMessage resultMessage;

    public TargetSelectionResult(GameObject selectedTarget, SearchResultMessage resultMessage)
        => (this.selectedTarget, selectedPosition, this.resultMessage) = (selectedTarget, selectedTarget.transform.position, resultMessage);

    public TargetSelectionResult(Vector3 selectedPosition, SearchResultMessage resultMessage)
        => (selectedTarget, this.selectedPosition, this.resultMessage) = (null, selectedPosition, resultMessage);
}
