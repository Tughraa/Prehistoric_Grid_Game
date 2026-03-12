using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonEffect : IStatusEffect
{
    public float effectDuration;
    public float effectStrength;


    public string GetMessage => ("You feel poisoned");
    public string GetName => ("poison");
    public Color GetColor => new Color(0.3f,0.67f,0.0f,0.65f);
    public bool hasParticles => true;
    public bool IsFinished => (effectDuration <= 0f);
    public float remainingDuraton => effectDuration;

    public float applyDamagePeriod = 0.75f;
    float damageTimer = 0f;
    
    public PoisonEffect(float duration, float strength, float period)
    {
        effectDuration = duration;
        effectStrength = strength;
        applyDamagePeriod = period;
    }
    
    public IStatusEffect Clone()
    {
        return new PoisonEffect(effectDuration, effectStrength, applyDamagePeriod);
    }

    public void RefreshFrom(IStatusEffect other)
    {
        if (other is not PoisonEffect incoming)
        {    
            return;
        }
        effectDuration = Mathf.Max(effectDuration, incoming.effectDuration);
        effectStrength = Mathf.Max(effectStrength, incoming.effectStrength);
        applyDamagePeriod = Mathf.Min(applyDamagePeriod, incoming.applyDamagePeriod);
    }

    public void OnApply(EntityGeneral entity)
    {
        
    }
    public void OnRemove(EntityGeneral entity)
    {
        
    }
    public void Tick(EntityGeneral entity, float deltaTime)
    {
        damageTimer += deltaTime;
        if (damageTimer >= applyDamagePeriod)
        {
            damageTimer = 0f;
            entity.Damage(effectStrength,"poison");
        }
        effectDuration -= deltaTime;
    }
}
