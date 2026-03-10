using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeEffect : IStatusEffect
{
    public float effectDuration;
    public float effectStrength;

    public string GetMessage => ("It's numbingly cold.");
    public Color GetColor => new Color(0.85f,0.89f,0.98f,0.85f);
    public bool hasParticles => true;
    public bool IsFinished => (effectDuration <= 0f);
    
    public FreezeEffect(float duration, float strength)
    {
        effectDuration = duration;
        effectStrength = strength;
    }
    public IStatusEffect Clone()
    {
        return new FreezeEffect(effectDuration, effectStrength);
    }

    public void RefreshFrom(IStatusEffect other)
    {
        if (other is not FreezeEffect incoming)
        {    
            return;
        }
        effectDuration = Mathf.Max(effectDuration, incoming.effectDuration);
        effectStrength = Mathf.Max(effectStrength, incoming.effectStrength);
    }

    public void OnApply(EntityGeneral entity)
    {
        entity.rigid.velocity /= 1.4f; 
        entity.entityStatusEffects.speedMults.Add(1f/effectStrength); //add fire immunity
        entity.entityStatusEffects.jumpForceMults.Add(0.8f);
        entity.damageImmunities.Add("fire");
    }
    public void OnRemove(EntityGeneral entity)
    {
        //entity.rigid.velocity = new Vector2(entity.rigid.velocity.x,entity.rigid.velocity.y+10f);

        entity.entityStatusEffects.speedMults.Remove(1f/effectStrength);
        entity.entityStatusEffects.jumpForceMults.Remove(0.8f);
        entity.damageImmunities.Remove("fire");
    }
    public void Tick(EntityGeneral entity, float deltaTime)
    {
        effectDuration -= deltaTime;
    }
}
