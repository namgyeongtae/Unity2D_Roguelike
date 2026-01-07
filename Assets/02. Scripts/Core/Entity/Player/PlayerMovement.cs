using System.Collections;
using UnityEditor;
using UnityEngine;


public class PlayerMovement : EntityMovement
{
    private float _dashTime = 0.5f;
    [SerializeField] private float _dodgeTime = 1.0f;
    [SerializeField] private float _dodgeDistance = 3.0f;

    public bool IsDashing { get; private set; }
    public bool IsDodging { get; private set; }

    public float MoveSpeed => _moveSpeedStat.Value;

    public void Dodge(Vector2 dir)
    {
        StartCoroutine(DodgeUpdate(dir));
    }

    private IEnumerator DodgeUpdate(Vector2 dir)
    {
        IsDodging = true;

        float currentRollTime = 0f;

        float prevRollDist = 0f;

        while (currentRollTime < _dodgeTime)
        {
            currentRollTime += Time.deltaTime;

            float timePoint = currentRollTime / _dodgeTime;

            float inOutSine = -(Mathf.Cos(Mathf.PI * timePoint) - 1) / 2;
            float currentRollDist = Mathf.Lerp(0f, _dodgeDistance, inOutSine);

            float deltaValue = currentRollDist - prevRollDist;

            Owner.transform.position += new Vector3(dir.x, dir.y, 0) * deltaValue;
            prevRollDist = currentRollDist;

            yield return null;
        }

        IsDodging = false;
    }
}
