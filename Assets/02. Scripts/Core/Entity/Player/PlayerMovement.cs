using UnityEditor;
using UnityEngine;


public class PlayerMovement : EntityMovement
{
    private float _dashTime = 0.5f;

    public bool IsDashing { get; private set; }

    public float MoveSpeed => _moveSpeedStat.Value;
}
