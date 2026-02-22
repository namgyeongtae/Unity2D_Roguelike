using System.Collections;
using UnityEngine;

[System.Serializable]
public class KnockbackAction : DamageAction
{
    [SerializeField] private float _distance = 1f;
    [SerializeField] private float _duration = 0.3f;

    public override IEnumerator OnDamage(Entity entity, Entity instigator, object causer, float damage)
    {
        yield return KnockbackCoroutine(entity, instigator);
    }

    private IEnumerator KnockbackCoroutine(Entity entity, Entity instigator)
    {
        // instigator와의 방향 계산 (instigator에서 플레이어로의 방향)
        Vector3 directionToPlayer = (entity.transform.position - instigator.transform.position).normalized;
        
        // 목표 위치 계산
        Vector3 startPosition = entity.transform.position;
        Vector3 targetPosition = startPosition + directionToPlayer * _distance;
        targetPosition.z = startPosition.z;
        
        // 넉백 애니메이션
        float elapsedTime = 0f;
        
        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _duration;
            
            // 이징 함수 적용 (예: ease-out)
            float easedT = 1f - Mathf.Pow(1f - t, 3f);
            
            // 위치 보간
            entity.transform.position = Vector3.Lerp(startPosition, targetPosition, easedT);
            
            yield return null;
        }

        // 최종 위치 보정
        entity.transform.position = new Vector3(targetPosition.x, targetPosition.y, entity.transform.position.z);
    }
}
