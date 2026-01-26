using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    [SerializeField] protected Stat _moveSpeedData;

    protected Vector2 _moveDir = Vector2.zero;

    protected Stat _moveSpeedStat;

    public Entity Owner { get; private set; }

    public Vector2 MoveDir => _moveDir;

    public virtual void Setup(Entity owner)
    {
        Owner = owner;

        _moveSpeedStat = _moveSpeedData ? Owner.Stats.GetStat(_moveSpeedData) : null;
    }

    public virtual void Move(Vector2 dir, float speedMultiplier = 1.0f)
    {
        _moveDir = dir;
        Owner.transform.position += (Vector3)_moveDir * _moveSpeedStat.Value * speedMultiplier * Time.deltaTime;
    }

    public void Stop()
    {
        _moveDir = Vector2.zero;
    }
}
