using System.Collections;
using UnityEngine;

[System.Serializable]
public class HitEffectAction : DamageAction
{
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private Color _targetColor = Color.red;

    private SpriteRenderer _spriteRenderer;

    public override IEnumerator OnDamage(Entity entity, Entity instigator, object causer, float damage)
    {
        yield return HitEffectCoroutine(entity);
    }

    private IEnumerator HitEffectCoroutine(Entity entity)
    {
        _spriteRenderer = entity.GetComponentInChildren<SpriteRenderer>();

        float elapsedTime = 0;

        Color originColor = Color.white;
        Color targetColor = _targetColor;

        Color[] colors = { originColor, targetColor };

        int currentColorIndex = 0;
        while (elapsedTime <= _duration)
        {
            elapsedTime += Time.deltaTime;

            _spriteRenderer.color = colors[currentColorIndex];

            currentColorIndex = (currentColorIndex + 1) % colors.Length;

            yield return null;
        }

        _spriteRenderer.color = originColor;
    }
}
