using System.Collections;
using UnityEngine;

[System.Serializable]
public class DamageAction
{
    public virtual IEnumerator OnDamage(Entity entity, Entity instigator, object causer, float damage) { yield break; }
}
