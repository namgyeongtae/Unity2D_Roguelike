using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    public delegate void OnChangedMoveDirHandler();

    [SerializeField] protected Stat _moveSpeedData;

    protected Vector2 _moveDir = Vector2.zero;

    protected Stat _moveSpeedStat;

    public Entity Owner { get; private set; }

    public Vector2 MoveDir => _moveDir;

    public event OnChangedMoveDirHandler onChangedMoveDir;

    public virtual void Setup(Entity owner)
    {
        Owner = owner;

        _moveSpeedStat = _moveSpeedData ? Owner.Stats.GetStat(_moveSpeedData) : null;
    }

    public virtual void Move(Vector2 dir)
    {
        if (_moveDir != Vector2.zero 
            && dir != Vector2.zero 
            && _moveDir != dir)
            return;

        Debug.Log("Move");

        _moveDir = dir;
        Owner.transform.position += (Vector3)_moveDir * _moveSpeedStat.Value * Time.deltaTime;
    }

    public void Stop()
    {
        _moveDir = Vector2.zero;
    }
}
