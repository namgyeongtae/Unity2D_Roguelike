using UnityEditor.MPE;
using UnityEngine;

[System.Serializable]
public abstract class SelectTarget : TargetSelectionAction
{
    [Header("Data")]
    [Min(0f)]
    [SerializeField]
    private float range; // 범위가 0일 경우 무한대를 의미함

    [Range(0f, 360f)]
    [SerializeField]
    private float angle;

    private TargetSearcher targetSearcher;
    private Entity requesterEntity;
    private GameObject requesterObject;
    private SelectCompletedHandler onSelectCompleted;

    public override object Range => range;
    public override object ScaledRange => range * Scale;
    public override float Angle => angle;

    public SelectTarget() { }

    public SelectTarget(SelectTarget copy) : base(copy) 
    {
        range = copy.range;
        angle = copy.angle;
    }

    protected abstract TargetSelectionResult SelectImmediateByPlayer(Vector2 screenPosition, TargetSearcher targetSearcher, Entity requesterEntity,
        GameObject requesterObject);

    protected sealed override TargetSelectionResult SelectImmediateByPlayer(TargetSearcher targetSearcher, Entity requesterEntity, 
        GameObject requesterObject, Vector3 position)
        => SelectImmediateByPlayer(Camera.main.WorldToScreenPoint(position), targetSearcher, requesterEntity, requesterObject);
    
    private void ResetMouseController()
    {
        MouseController.Instance.ChangeCursor(CursorType.Default);
        MouseController.Instance.onLeftClick -= OnMouseLeftClick;
        MouseController.Instance.onRightClick -= OnMouseRightClick;
    }

    public override void Select(TargetSearcher targetSearcher, Entity requesterEntity, 
        GameObject requesterObject, SelectCompletedHandler onSelectCompleted)
    {
        if (requesterEntity.IsPlayer)
        {
            this.targetSearcher = targetSearcher;
            this.requesterEntity = requesterEntity;
            this.requesterObject = requesterObject;
            this.onSelectCompleted = onSelectCompleted;
            
            MouseController.Instance.ChangeCursor(CursorType.BlueArrow);
            MouseController.Instance.onLeftClick += OnMouseLeftClick;
            MouseController.Instance.onRightClick += OnMouseRightClick;
        }
        else
        {
            onSelectCompleted.Invoke(SelectImmediateByAI(targetSearcher, requesterEntity, requesterObject, requesterEntity.Target.transform.position));
        }
    }

    public override void CancelSelect(TargetSearcher targetSearcher)
    {
        ResetMouseController();
    }

    public override bool IsInRange(TargetSearcher targetSearcher, Entity requesterEntity, GameObject requesterObject, Vector3 targetPosition)
    {
        var requesterTransform = requesterObject.transform;
        targetPosition.z = requesterTransform.position.z;

        float sqrRange = range * range * (IsUseScale ? Scale : 1f);
        Vector3 relativePosition = targetPosition - requesterTransform.position;
        float angle = Vector3.Angle(relativePosition, requesterTransform.forward);
        bool IsInAngle = angle <= (Angle / 2f);

        return Mathf.Approximately(0f, range) || 
            (Vector3.SqrMagnitude(relativePosition) <= sqrRange && IsInAngle);
    }

    private void OnMouseLeftClick(Vector2 mousePosition)
    {
        ResetMouseController();

        onSelectCompleted?.Invoke(SelectImmediateByPlayer(mousePosition, targetSearcher, requesterEntity, requesterObject));
    }

    private void OnMouseRightClick(Vector2 mousePosition)
    {
        ResetMouseController();

        // 결과 값으로 실패를 Delegate로 전달
        onSelectCompleted?.Invoke(new TargetSelectionResult(Vector2.zero, SearchResultMessage.Fail));
    }
}
