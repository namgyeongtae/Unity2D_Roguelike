using System.Collections;
using UnityEditor;
using UnityEngine;


public class PlayerMovement : EntityMovement
{    
    [SerializeField] private float _dodgeTime = 0.01f;
    [SerializeField] private float _dodgeDistance = 0.5f;

    [SerializeField] private float _dodgeSpeed = 3f;

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

            float currentRollDist = Mathf.Lerp(0f, _dodgeDistance, timePoint * _dodgeSpeed);

            float deltaValue = currentRollDist - prevRollDist;

            Owner.transform.position += new Vector3(dir.x, dir.y, 0) * deltaValue;
            prevRollDist = currentRollDist;

            Debug.Log($"currentRollTime: {currentRollTime}, dodgeTime: {_dodgeTime}");

            yield return null;
        }

        IsDodging = false;
    }
}
