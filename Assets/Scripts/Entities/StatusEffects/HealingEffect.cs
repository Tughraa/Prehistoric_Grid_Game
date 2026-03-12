using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingEffect : IStatusEffect
{
    public float effectDuration;
    public float effectStrength;

    public Color GetColor => new Color(0.8f,0.24f,0.15f,0.85f);
    public string GetMessage => ("You're feeling healthier");
    public bool hasParticles => true;
    public bool IsFinished => (effectDuration <= 0f);
    public float remainingDuraton => effectDuration;

    public float applyHealPeriod = 0.75f;
    public float healTimer = 0f;
    
    public HealingEffect(float duration, float strength, float period)
    {
        effectDuration = duration;
        effectStrength = strength;
        applyHealPeriod = period;
    }
    public IStatusEffect Clone()
    {
        return new HealingEffect(effectDuration, effectStrength, applyHealPeriod);
    }

    public void RefreshFrom(IStatusEffect other)
    {
        if (other is not HealingEffect incoming)
        {    
            return;
        }
        effectDuration = Mathf.Max(effectDuration, incoming.effectDuration);
        effectStrength = Mathf.Max(effectStrength, incoming.effectStrength);
        applyHealPeriod = Mathf.Min(applyHealPeriod, incoming.applyHealPeriod);
    }

    public void OnApply(EntityGeneral entity)
    {
        
    }
    public void OnRemove(EntityGeneral entity)
    {
        
    }
    public void Tick(EntityGeneral entity, float deltaTime)
    {
        effectDuration -= deltaTime;

        healTimer += deltaTime;
        if (healTimer >= applyHealPeriod)
        {
            healTimer = 0f;
            entity.Heal(effectStrength);
        }
    }
}
