using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffect : IStatusEffect
{
    public float effectDuration;
    public float effectStrength;

    public string GetMessage => ("You can't feel your body");
    public string GetName => ("stun");
    public Color GetColor => new Color(0.93f,0.74f,0.3f,0.85f);
    public bool hasParticles => true;
    public bool IsFinished => (effectDuration <= 0f);
    public float remainingDuraton => effectDuration;
    
    public StunEffect(float duration, float strength)
    {
        effectDuration = duration;
        effectStrength = strength;
    }
    public IStatusEffect Clone()
    {
        return new StunEffect(effectDuration, effectStrength);
    }

    public void RefreshFrom(IStatusEffect other)
    {
        if (other is not StunEffect incoming)
        {    
            return;
        }
        effectDuration = Mathf.Max(effectDuration, incoming.effectDuration);
        effectStrength = Mathf.Max(effectStrength, incoming.effectStrength);
    }

    public void OnApply(EntityGeneral entity)
    {
        entity.rigid.velocity = entity.rigid.velocity/3f;
        entity.entityStatusEffects.speedMults.Add(1f/effectStrength);
        entity.entityStatusEffects.jumpForceMults.Add(3f/effectStrength);
    }
    public void OnRemove(EntityGeneral entity)
    {
        entity.rigid.velocity = new Vector2(0f,3f);

        entity.entityStatusEffects.speedMults.Remove(1f/effectStrength);
        entity.entityStatusEffects.jumpForceMults.Remove(3f/effectStrength);
        effectDuration = -2f;
    }
    public void Tick(EntityGeneral entity, float deltaTime)
    {
        effectDuration -= deltaTime;
    }
}
