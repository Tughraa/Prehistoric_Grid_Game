using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoostEffect : IStatusEffect
{
    public float effectDuration;
    public float effectStrength;

    public string GetMessage => ("Your legs feel much stronger");
    public Color GetColor => new Color(0.02f,0.86f,0.25f,0.85f);
    public bool hasParticles => true;
    public bool IsFinished => (effectDuration <= 0f);
    
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
    }
    public void OnRemove(EntityGeneral entity)
    {
        entity.entityStatusEffects.jumpForceMults.Remove(effectStrength);
        //entity.rigid.velocity = new Vector2(entity.rigid.velocity.x/2f,entity.rigid.velocity.y);
    }
    public void Tick(EntityGeneral entity, float deltaTime)
    {
        effectDuration -= deltaTime;
    }
}
