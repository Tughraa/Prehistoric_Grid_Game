using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatEffect : IStatusEffect
{
    public float effectDuration;
    public float effectStrength;

    public Color GetColor => new Color(0.7f,0.8f,0.89f,0.65f);
    public string GetMessage => ("You feel lighter than a feather...");
    public bool hasParticles => true;
    public bool IsFinished => (effectDuration <= 0f);
    
    public FloatEffect(float duration, float strength)
    {
        effectDuration = duration;
        effectStrength = strength;
    }
    public IStatusEffect Clone()
    {
        return new FloatEffect(effectDuration, effectStrength);
    }

    public void RefreshFrom(IStatusEffect other)
    {
        if (other is not FloatEffect incoming)
        {    
            return;
        }
        effectDuration = Mathf.Max(effectDuration, incoming.effectDuration);
        effectStrength = Mathf.Max(effectStrength, incoming.effectStrength);
    }

    public void OnApply(EntityGeneral entity)
    {
        entity.entityStatusEffects.gravityScaleMults.Add(-effectStrength);
    }
    public void OnRemove(EntityGeneral entity)
    {
        entity.entityStatusEffects.gravityScaleMults.Remove(-effectStrength);
    }
    public void Tick(EntityGeneral entity, float deltaTime)
    {
        effectDuration -= deltaTime;
    }
}
