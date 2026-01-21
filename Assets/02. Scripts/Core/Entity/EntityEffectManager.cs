using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class EntityEffectManager : MonoBehaviour
{
    private readonly List<Effect> effects = new();

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            var effect = effects[i];

            if (!effect.IsFinished)
                effect.Update();
            else
            {
                effect.Release();
                effects.Remove(effect);
            }
        }
    }

    public void AddEffect(Effect effect)
    {
        effects.Add(effect);
        effect.Start();

        if (effect.IsApplicable)
            effect.Apply();
    }
}
