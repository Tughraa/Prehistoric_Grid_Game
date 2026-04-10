using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoostEffect : IStatusEffect
{
    public float effectDuration;
    public float effectStrength;

    public string GetMessage => ("Your legs feel much stronger");
    public string GetName => ("jumpBoost");
    public Color GetColor => new Color(0.02f,0.86f,0.25f,0.85f);
    public bool hasParticles => true;
    public bool IsFinished => (effectDuration <= 0f);
    public float remainingDuraton => effectDuration;
    
    public JumpBoostEffect(float duration, float strength)
    {
        effectDuration = duration;
        effectStrength = strength;
    }
    
    public IStatusEffect Clone()
    {
        return new JumpBoostEffect(effectDuration, effectStrength);
    }

    public void RefreshFrom(IStatusEffect other)
    {
        if (other is not SpeedEffect incoming)
        {    
            return;
        }
        effectDuration = Mathf.Max(effectDuration, incoming.effectDuration);
        effectStrength = Mathf.Max(effectStrength, incoming.effectStrength);
    }

    public void OnApply(EntityGeneral entity)
    {
        entity.entityStatusEffects.jumpForceMults.Add(effectStrength);
        entity.damageImmunities.Add("fall");
    }
    public void OnRemove(EntityGeneral entity)
    {
        entity.entityStatusEffects.jumpForceMults.Remove(effectStrength);
        //entity.rigid.velocity = new Vector2(entity.rigid.velocity.x/2f,entity.rigid.velocity.y);
        effectDuration = -2f;
        entity.damageImmunities.Remove("fall");
    }
    public void Tick(EntityGeneral entity, float deltaTime)
    {
        effectDuration -= deltaTime;
    }
}
